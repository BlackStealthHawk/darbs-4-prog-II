using Godot;

public partial class Bullet : Area2D
{
	[Export] public float Speed { get; set; } = 700.0f;
	[Export] public int Damage { get; set; } = 10;
	[Export] public float MaxLifetime { get; set; } = 2.0f;

	private Vector2 direction = Vector2.Right;
	private float lifetime;
	public bool IsActive { get; private set; }
	public bool UsePool { get; private set; }

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsActive) return;

		GlobalPosition += direction * Speed * (float)delta;
		lifetime -= (float)delta;
		if (lifetime <= 0.0f)
		{
			if (UsePool) Deactivate();
			else QueueFree();
		}
	}

	public void Fire(Vector2 startPosition, Vector2 moveDirection, bool fromPool)
	{
		GlobalPosition = startPosition;
		direction = moveDirection.Normalized();
		Rotation = direction.Angle();
		lifetime = MaxLifetime;
		UsePool = fromPool;
		SetActiveState(true);
	}

	public void Deactivate()
	{
		SetActiveState(false);
	}

	public void SetActiveState(bool active)
	{
		IsActive = active;
		Visible = active;
		SetPhysicsProcess(active);
		SetDeferred(Area2D.PropertyName.Monitoring, active);
	}

	private void OnBodyEntered(Node2D body)
	{
		if (!IsActive || body is Player) return;

		if (body is Enemy enemy)
		{
			enemy.TakeDamage(Damage);
		}

		if (UsePool) Deactivate();
		else QueueFree();
	}
}
