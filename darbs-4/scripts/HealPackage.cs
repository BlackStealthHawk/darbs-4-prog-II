using Godot;

public partial class HealPackage : Area2D
{
	private bool collected;

	public override void _Ready()
	{
		CollisionShape2D collision = new()
		{
			Name = "CollisionShape2D",
			Shape = new CircleShape2D { Radius = 14.0f }
		};
		AddChild(collision);

		Polygon2D package = new()
		{
			Name = "Package",
			Color = new Color(0.15f, 0.8f, 0.25f),
			Polygon = new Vector2[]
			{
				new(-10, -10), new(10, -10), new(10, 10), new(-10, 10)
			}
		};
		AddChild(package);

		Polygon2D cross = new()
		{
			Name = "Cross",
			Color = Colors.White,
			Polygon = new Vector2[]
			{
				new(-2, -7), new(2, -7), new(2, -2), new(7, -2),
				new(7, 2), new(2, 2), new(2, 7), new(-2, 7),
				new(-2, 2), new(-7, 2), new(-7, -2), new(-2, -2)
			}
		};
		AddChild(cross);

		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (collected || body is not Player player || !player.IsAlive) return;

		collected = true;
		player.Heal(player.MaxHp);
		GD.Print($"[HealPackage] Spēlētājs atjaunoja HP: {player.Hp}/{player.MaxHp}");
		QueueFree();
	}
}
