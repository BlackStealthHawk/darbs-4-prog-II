using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 240.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 input = Vector2.Zero;

		if (Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left))
			input.X -= 1.0f;
		if (Input.IsKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right))
			input.X += 1.0f;
		if (Input.IsKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up))
			input.Y -= 1.0f;
		if (Input.IsKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down))
			input.Y += 1.0f;

		if (input != Vector2.Zero)
			input = input.Normalized();

		Velocity = input * Speed;
		MoveAndSlide();
	}
}
