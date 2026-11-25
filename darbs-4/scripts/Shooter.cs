using Godot;

public partial class Shooter : Enemy
{
	private const int BulletDamage = 25;

	[Export] public PackedScene BulletScene { get; set; } = GD.Load<PackedScene>("res://scenes/bullet.tscn");
	[Export] public float BulletSpeed { get; set; } = 450.0f;
	[Export] public float BulletSpawnOffset { get; set; } = 24.0f;

	public Shooter()
	{
		EnemyName = "Shooter";
		Speed = 115.0f;
		Hp = 20;
		DetectRange = 480.0f;
		AttackRange = 240.0f;
		AttackDamage = BulletDamage;
		AttackInterval = 1.2f;
		ScoreValue = 150;
		BodyColor = new Color(0.68f, 0.32f, 0.92f);
	}

	protected override void PerformAttack(double delta)
	{
		if (player == null || !player.IsAlive) return;

		LookAt(player.GlobalPosition);
		attackCooldown -= delta;

		if (attackCooldown <= 0.0)
		{
			attackCooldown = AttackInterval;
			ShootAtPlayer();
		}
	}

	private void ShootAtPlayer()
	{
		if (BulletScene == null)
		{
			GD.PushError($"[{EnemyName}] Nav piešķirta lodes aina.");
			return;
		}

		Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
		Bullet bullet = BulletScene.Instantiate<Bullet>();
		GetParent().AddChild(bullet);
		bullet.Speed = BulletSpeed;
		bullet.Damage = BulletDamage;
		bullet.Fire(
			GlobalPosition + direction * BulletSpawnOffset,
			direction,
			fromPool: false,
			hitPlayer: true,
			damage: BulletDamage);
		GD.Print($"[{EnemyName}] Izšāva uz spēlētāju.");
	}
}
