using Godot;
using System.Collections.Generic;

public record WavePlan(int ChaserCount, int ShooterCount);

public partial class WaveManager : Node
{
	private readonly List<Enemy> activeEnemies = new();
	private readonly List<Enemy> defeatedBuffer = new();

	private readonly Dictionary<string, int> stats = new()
	{
		["wave"] = 0,
		["score"] = 0,
		["defeated"] = 0
	};

	private readonly List<WavePlan> waves = new()
	{
		new WavePlan(2, 1), // 1. vilnis: 3 ienaidnieki
		new WavePlan(3, 2), // 2. vilnis: 5 ienaidnieki
		new WavePlan(4, 3)  // 3. vilnis: 7 ienaidnieki
	};

	private readonly Vector2[] spawnPoints = new Vector2[]
	{
		new(250, 70),
		new(980, 70),
		new(1150, 400),
		new(500, 100),
		new(600, 600),
		new(600, 100)
	};

	private Player player;
	private Label statusLabel;
	private Label messageLabel;
	private NavigationRegion2D navigationRegion;
	private Vector2[] arenaOutline;
	private Rect2 movementBounds;
	private readonly RandomNumberGenerator random = new();
	private bool gameEnded = false;

	public override void _Ready()
	{
		Node arena = GetParent();
		player = arena.GetNodeOrNull<Player>("Player");
		statusLabel = arena.GetNodeOrNull<Label>("HUD/StatusLabel");
		messageLabel = arena.GetNodeOrNull<Label>("HUD/MessageLabel");
		navigationRegion = arena.GetNodeOrNull<NavigationRegion2D>("NavigationRegion2D");

		if (player == null)
		{
			GD.PushError("WaveManager: Arena nav atrasts Player mezgls.");
			return;
		}

		if (navigationRegion?.NavigationPolygon == null || navigationRegion.NavigationPolygon.GetOutlineCount() == 0)
		{
			GD.PushError("WaveManager: NavigationRegion2D nav derīgas arēnas robežas.");
			return;
		}

		arenaOutline = navigationRegion.NavigationPolygon.GetOutline(0);
		Vector2 firstPoint = navigationRegion.ToGlobal(arenaOutline[0]);
		Vector2 min = firstPoint;
		Vector2 max = firstPoint;
		for (int i = 0; i < arenaOutline.Length; i++)
		{
			Vector2 worldPoint = navigationRegion.ToGlobal(arenaOutline[i]);
			arenaOutline[i] = worldPoint;
			min = new Vector2(Mathf.Min(min.X, worldPoint.X), Mathf.Min(min.Y, worldPoint.Y));
			max = new Vector2(Mathf.Max(max.X, worldPoint.X), Mathf.Max(max.Y, worldPoint.Y));
		}

		movementBounds = new Rect2(min, max - min).Grow(-24.0f);
		random.Randomize();
		CallDeferred(nameof(StartNextWave));
	}

	public override void _Process(double delta)
	{
		if (gameEnded && Input.IsActionJustPressed("restart"))
		{
			GetTree().ReloadCurrentScene();
			return;
		}

		if (gameEnded) return;

		if (player != null && !player.IsAlive)
		{
			gameEnded = true;
			SetMessage("SPĒLE BEIGUSIES! Nospied R, lai mēģinātu vēlreiz.");
			UpdateHud();
			return;
		}

		RemoveDefeatedEnemies();

		if (activeEnemies.Count == 0)
		{
			StartNextWave();
		}

		UpdateHud();
	}

	private void RemoveDefeatedEnemies()
	{
		defeatedBuffer.Clear();

		foreach (Enemy enemy in activeEnemies)
		{
			if (!IsInstanceValid(enemy) || !enemy.IsAlive)
			{
				defeatedBuffer.Add(enemy);
			}
		}

		foreach (Enemy defeated in defeatedBuffer)
		{
			activeEnemies.Remove(defeated);
			if (IsInstanceValid(defeated))
			{
				stats["score"] += defeated.ScoreValue;
				stats["defeated"] += 1;
				defeated.QueueFree();
			}
		}
	}

	private void StartNextWave()
	{
		int nextWaveIndex = stats["wave"];
		if (nextWaveIndex >= waves.Count)
		{
			gameEnded = true;
			SetMessage($"UZVARA! Visi {waves.Count} viļņi pieveikti! Punkti: {stats["score"]} (R — sākt no jauna)");
			UpdateHud();
			return;
		}

		WavePlan plan = waves[nextWaveIndex];
		stats["wave"] = nextWaveIndex + 1;

		int spawnIndex = 0;
		for (int i = 0; i < plan.ChaserCount; i++)
		{
			SpawnEnemy(new Chaser(), GetSpawnPosition(spawnIndex));
			spawnIndex++;
		}

		for (int i = 0; i < plan.ShooterCount; i++)
		{
			SpawnEnemy(new Shooter(), GetSpawnPosition(spawnIndex));
			spawnIndex++;
		}

		if (stats["wave"] == 2)
		{
			SpawnHealPackage(GetSpawnPosition(spawnIndex));
		}

		if (nextWaveIndex == waves.Count - 1)
		{
			SpawnEnemy(new UltraEnemy(), GetSpawnPosition(spawnIndex));
		}

		string pickupMessage = stats["wave"] == 2 ? " | Arēnā ir pilnas dzīvības komplekts!" : "";
		SetMessage($"Vilnis {stats["wave"]}/{waves.Count} — Ienaidnieki: {activeEnemies.Count}{pickupMessage}");
		UpdateHud();
	}

	private void SpawnHealPackage(Vector2 position)
	{
		HealPackage package = new();
		GetParent().AddChild(package);
		package.GlobalPosition = position;
	}

	private void SpawnEnemy(Enemy enemy, Vector2 pos)
	{
		GetParent().AddChild(enemy);
		enemy.Setup(player, pos);
		enemy.SetMovementBounds(movementBounds);
		activeEnemies.Add(enemy);
	}

	private Vector2 GetSpawnPosition(int index)
	{
		Vector2 basePosition = spawnPoints[index % spawnPoints.Length];
		for (int attempt = 0; attempt < 30; attempt++)
		{
			Vector2 candidate = basePosition + new Vector2(
				random.RandfRange(-35.0f, 35.0f),
				random.RandfRange(-35.0f, 35.0f));

			if (!movementBounds.HasPoint(candidate)
				|| !Geometry2D.IsPointInPolygon(candidate, arenaOutline)
				|| candidate.DistanceTo(player.GlobalPosition) < 240.0f)
			{
				continue;
			}

			bool tooCloseToOtherEnemy = false;
			foreach (Enemy activeEnemy in activeEnemies)
			{
				if (IsInstanceValid(activeEnemy) && candidate.DistanceTo(activeEnemy.GlobalPosition) < 80.0f)
				{
					tooCloseToOtherEnemy = true;
					break;
				}
			}

			if (!tooCloseToOtherEnemy) return candidate;
		}

		return basePosition;
	}

	private void UpdateHud()
	{
		if (statusLabel == null) return;

		int hp = player != null ? player.Hp : 0;
		int maxHp = player != null ? player.MaxHp : 100;

		statusLabel.Text =
			$"Vilnis: {stats["wave"]}/{waves.Count} | " +
			$"Aktīvie ienaidnieki: {activeEnemies.Count} | " +
			$"Uzveikti: {stats["defeated"]} | " +
			$"Punkti: {stats["score"]} | " +
			$"HP: {hp}/{maxHp}";
	}

	private void SetMessage(string text)
	{
		if (messageLabel != null)
		{
			messageLabel.Text = text;
		}
		GD.Print($"[WaveManager] {text}");
	}
}
