using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 240.0f;
	[Export] public int MaxHp { get; set; } = 100;
	public int Hp { get; private set; } = 100;
	public bool IsAlive => Hp > 0;
	[Export] public PackedScene BulletScene { get; set; } = GD.Load<PackedScene>("res://scenes/bullet.tscn");
	[Export] public bool UsePooling { get; set; } = true;
	[Export] public int PoolSize { get; set; } = 50;
	[Export] public float FireCooldown { get; set; } = 0.02f;

	private readonly BulletPool bulletPool = new();
	private double cooldownTimer = 0.0;

	public override void _Ready()
	{
		Hp = MaxHp;
		CallDeferred(nameof(SetupPool));
	}

	private void SetupPool()
	{
		bulletPool.Initialize(GetParent(), BulletScene, PoolSize);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsAlive)
		{
			Velocity = Vector2.Zero;
			return;
		}

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (inputDir == Vector2.Zero)
		{
			if (Input.IsPhysicalKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left))
				inputDir.X -= 1.0f;
			if (Input.IsPhysicalKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right))
				inputDir.X += 1.0f;
			if (Input.IsPhysicalKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up))
				inputDir.Y -= 1.0f;
			if (Input.IsPhysicalKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down))
				inputDir.Y += 1.0f;
		}

		if (inputDir != Vector2.Zero)
		{
			inputDir = inputDir.Normalized();
		}

		Velocity = inputDir * Speed;
		MoveAndSlide();
		Vector2 aimDirection = GetGlobalMousePosition() - GlobalPosition;
		if (aimDirection != Vector2.Zero)
		{
			GlobalRotation = aimDirection.Angle() + Mathf.Pi / 2.0f;
		}

		cooldownTimer -= delta;
		if (Input.IsActionPressed("shoot") && cooldownTimer <= 0.0)
		{
			Shoot();
			cooldownTimer = FireCooldown;
		}
	}

	private void Shoot()
	{
		if (BulletScene == null) return;

		Vector2 bulletDirection = -GlobalTransform.Y.Normalized();
		Vector2 spawnPosition = GlobalPosition + bulletDirection * 40.0f;

		if (UsePooling)
		{
			Bullet pooledBullet = bulletPool.GetBullet();
			if (pooledBullet == null) return;

			pooledBullet.Fire(spawnPosition, bulletDirection, true);
		}
		else
		{
			Bullet newBullet = BulletScene.Instantiate<Bullet>();
			GetParent().AddChild(newBullet);
			newBullet.Fire(spawnPosition, bulletDirection, false);
		}
	}

	public void TakeDamage(int amount)
	{
		if (!IsAlive || amount <= 0) return;

		Hp = Mathf.Max(0, Hp - amount);
		GD.Print($"[Player] Saņemti -{amount} bojājumi. HP: {Hp}/{MaxHp}");

		if (Hp == 0)
		{
			Modulate = new Color(0.4f, 0.4f, 0.4f, 0.7f);
			GD.Print("[Player] Spēlētājs ir zaudējis visus HP!");
		}
	}

	public void Heal(int amount)
	{
		if (!IsAlive || amount <= 0) return;
		Hp = Mathf.Min(MaxHp, Hp + amount);
	}
}
