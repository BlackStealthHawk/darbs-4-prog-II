using Godot;

public enum EnemyState
{
	Idle,
	Patrol,
	Chase,
	Attack,
	Dead
}

public partial class Enemy : CharacterBody2D
{
	[Export] public string EnemyName { get; set; } = "Enemy";
	[Export] public float Speed { get; set; } = 130.0f;
	[Export] public int Hp { get; set; } = 30;
	[Export] public bool UsePathfinding { get; set; } = true;
	[Export] public float DetectRange { get; set; } = 420.0f;
	[Export] public float AttackRange { get; set; } = 48.0f;
	[Export] public int AttackDamage { get; set; } = 10;
	[Export] public float AttackInterval { get; set; } = 0.9f;
	[Export] public int ScoreValue { get; set; } = 100;
	[Export] public Color BodyColor { get; set; } = new Color(0.9f, 0.25f, 0.25f);

	public EnemyState CurrentState { get; private set; } = EnemyState.Idle;
	public bool IsAlive => CurrentState != EnemyState.Dead && Hp > 0;

	protected Player player;
	protected NavigationAgent2D navAgent;
	protected double attackCooldown = 0.0;

	private Vector2 patrolOrigin;
	private int patrolDirection = 1;
	private Rect2 movementBounds;
	private bool hasMovementBounds;

	public override void _Ready()
	{
		EnsureComponents();
		patrolOrigin = GlobalPosition;
		player = GetParent()?.GetNodeOrNull<Player>("Player");
		navAgent = GetNodeOrNull<NavigationAgent2D>("NavigationAgent2D");
		ChangeState(EnemyState.Patrol);
	}

	public void Setup(Player targetPlayer, Vector2 spawnPos)
	{
		player = targetPlayer;
		GlobalPosition = spawnPos;
		patrolOrigin = spawnPos;
	}

	public void SetMovementBounds(Rect2 bounds)
	{
		movementBounds = bounds;
		hasMovementBounds = true;
	}

	private void EnsureComponents()
	{
		if (GetNodeOrNull<CollisionShape2D>("CollisionShape2D") == null)
		{
			CollisionShape2D col = new() { Name = "CollisionShape2D" };
			col.Shape = new CircleShape2D { Radius = 16.0f };
			AddChild(col);
		}

		Polygon2D visual = GetNodeOrNull<Polygon2D>("Visual");
		if (visual == null)
		{
			visual = new Polygon2D
			{
				Name = "Visual",
				Polygon = new Vector2[]
				{
					new(-16, -16), new(16, -12), new(20, 0), new(16, 12), new(-16, 16)
				}
			};
			AddChild(visual);
		}
		visual.Color = BodyColor;

		if (GetNodeOrNull<NavigationAgent2D>("NavigationAgent2D") == null)
		{
			NavigationAgent2D agent = new()
			{
				Name = "NavigationAgent2D",
				PathDesiredDistance = 16.0f,
				TargetDesiredDistance = 20.0f
			};
			AddChild(agent);
		}
	}

	public void ChangeState(EnemyState newState)
	{
		if (CurrentState == EnemyState.Dead || CurrentState == newState) return;
		CurrentState = newState;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (CurrentState == EnemyState.Dead) return;

		if (player != null && player.IsAlive)
		{
			float dist = GlobalPosition.DistanceTo(player.GlobalPosition);
			if (dist < AttackRange)
			{
				ChangeState(EnemyState.Attack);
			}
			else if (dist < DetectRange)
			{
				ChangeState(EnemyState.Chase);
			}
			else
			{
				ChangeState(EnemyState.Patrol);
			}
		}
		else
		{
			ChangeState(EnemyState.Patrol);
		}

		switch (CurrentState)
		{
			case EnemyState.Idle:
				Velocity = Vector2.Zero;
				break;

			case EnemyState.Patrol:
				Velocity = new Vector2(patrolDirection * (Speed * 0.5f), 0);
				if (Mathf.Abs(GlobalPosition.X - patrolOrigin.X) > 90.0f)
				{
					patrolDirection *= -1;
				}
				break;

			case EnemyState.Chase:
				ChasePlayer();
				break;

			case EnemyState.Attack:
				Velocity = Vector2.Zero;
				PerformAttack(delta);
				break;
		}

		MoveAndSlide();

		if (hasMovementBounds)
		{
			Vector2 boundedPosition = GlobalPosition.Clamp(movementBounds.Position, movementBounds.End);
			if (boundedPosition != GlobalPosition)
			{
				GlobalPosition = boundedPosition;
				Velocity = Vector2.Zero;
			}
		}
	}

	protected virtual void ChasePlayer()
	{
		if (player == null) return;

		if (UsePathfinding && navAgent != null)
		{
			navAgent.TargetPosition = player.GlobalPosition;
			Vector2 nextPoint = navAgent.GetNextPathPosition();
			Vector2 dir = (nextPoint - GlobalPosition).Normalized();
			Velocity = dir * Speed;
			LookAt(nextPoint);
		}
		else
		{
			Vector2 dir = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = dir * Speed;
			LookAt(player.GlobalPosition);
		}
	}

	protected virtual void PerformAttack(double delta)
	{
		if (player == null || !player.IsAlive) return;

		LookAt(player.GlobalPosition);
		attackCooldown -= delta;

		if (attackCooldown <= 0.0)
		{
			attackCooldown = AttackInterval;
			player.TakeDamage(AttackDamage);
		}
	}

	public void TakeDamage(int amount)
	{
		if (CurrentState == EnemyState.Dead || amount <= 0) return;

		Hp -= amount;
		if (Hp <= 0)
		{
			Hp = 0;
			Velocity = Vector2.Zero;
			ChangeState(EnemyState.Dead);
			Modulate = new Color(0.35f, 0.35f, 0.35f, 0.65f);
			CollisionLayer = 0;
			CollisionMask = 0;
			SetPhysicsProcess(false);
		}
	}
}
