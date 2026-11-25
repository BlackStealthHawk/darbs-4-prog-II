using Godot;

public partial class Chaser : Enemy
{
	public Chaser()
	{
		EnemyName = "Chaser";
		Speed = 175.0f;
		Hp = 30;
		DetectRange = 420.0f;
		AttackRange = 48.0f;
		AttackDamage = 10;
		AttackInterval = 0.8f;
		ScoreValue = 100;
		BodyColor = new Color(0.92f, 0.24f, 0.24f);
	}
}
