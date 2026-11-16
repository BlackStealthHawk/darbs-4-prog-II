using Godot;

public enum EnemyState
{
	Idle,   // gaida
	Patrol, // patrulē
	Chase,  // dzenas pakaļ
	Attack, // uzbrūk
	Dead    // gala stāvoklis
}

public partial class Enemy : CharacterBody2D
{
	public string EnemyName = "Pretinieks";
	[Export] public float Speed = 140.0f;
	[Export] public int Hp = 30;

	protected EnemyState state = EnemyState.Patrol;
	protected Node2D player;

	public bool IsAlive => state != EnemyState.Dead;

	public override void _Ready()
	{
		player = GetParent()?.GetNodeOrNull<Node2D>("Player");
		GD.Print("Sākuma stāvoklis: ", state);
	}

	protected void ChangeState(EnemyState newState)
	{
		if (state == EnemyState.Dead || state == newState) return;
		GD.Print("Stāvokļa maiņa: ", state, " -> ", newState);
		state = newState;
	}

	public void TakeDamage(int amount)
	{
		if (!IsAlive || amount <= 0) return;
		Hp -= amount;
		GD.Print("Ienaidnieks saņēma ", amount, " bojājumus. HP: ", Hp);
		if (Hp <= 0)
		{
			Hp = 0;
			ChangeState(EnemyState.Dead);
			Velocity = Vector2.Zero;
			Modulate = new Color(0.5f, 0.5f, 0.5f, 0.6f);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsKeyPressed(Key.K))
			TakeDamage(30);

		if (player == null || !IsAlive) return;

		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);

		switch (state)
		{
			case EnemyState.Idle:
			case EnemyState.Patrol:
				Velocity = Vector2.Zero;
				if (dist < 300.0f) ChangeState(EnemyState.Chase);
				break;

			case EnemyState.Chase:
				Vector2 dir = (player.GlobalPosition - GlobalPosition).Normalized();
				Velocity = dir * Speed;
				MoveAndSlide();

				if (dist < 60.0f) ChangeState(EnemyState.Attack);
				else if (dist > 400.0f) ChangeState(EnemyState.Patrol);
				break;

			case EnemyState.Attack:
				Velocity = Vector2.Zero;
				if (dist >= 80.0f) ChangeState(EnemyState.Chase);
				break;

			case EnemyState.Dead:
				break; // Gala stāvoklis — izejošu pāreju nav.
		}
	}
}
