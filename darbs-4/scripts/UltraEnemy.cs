using Godot;

public partial class UltraEnemy : Enemy
{
	public UltraEnemy()
	{
		EnemyName = "Ultra";
		Speed = 145.0f;
		Hp = 150;
		DetectRange = 700.0f;
		AttackRange = 64.0f;
		AttackDamage = 99;
		AttackInterval = 1.0f;
		ScoreValue = 500;
		BodyColor = new Color(1.0f, 0.78f, 0.12f);
	}
}
