using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 240.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (input == Vector2.Zero)
		{
			if (Input.IsPhysicalKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left))
				input.X -= 1.0f;
			if (Input.IsPhysicalKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right))
				input.X += 1.0f;
			if (Input.IsPhysicalKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up))
				input.Y -= 1.0f;
			if (Input.IsPhysicalKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down))
				input.Y += 1.0f;

			if (input != Vector2.Zero)
			{
				input = input.Normalized();
			}
		}

		Velocity = input * Speed;
		MoveAndSlide();
	}
}
