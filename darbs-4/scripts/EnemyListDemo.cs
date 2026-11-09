using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyListDemo : Node
{
	private List<Enemy> enemies = new()
	{
		new Enemy("Pirmais"),
		new Enemy("Otrais"),
		new Enemy("Trešais"),
		new Enemy("Ceturtais")
	};

	public override void _Ready()
	{
		Godot.Collections.Array bullets = new();
		bullets.Add(10);
		bullets.Add(20);
		bullets.Add("rakete");
		GD.Print($"Ložu skaits: {bullets.Count}");

		List<int> numberedBullets = new()
		{
			10,
			20
		};
		GD.Print($"List<int> skaits: {numberedBullets.Count}");

		int index = 1;
		if (index >= 0 && index < enemies.Count)
		{
			enemies.RemoveAt(index);
			GD.Print($"Noņemts elements ar indeksu {index}; skaits: {enemies.Count}");
		}
		else
		{
			GD.Print($"Indekss {index} nav derīgs; skaits: {enemies.Count}");
		}

		List<Enemy> unsafeEnemies = new(enemies);
		try
		{
			foreach (Enemy enemy in unsafeEnemies)
			{
				if (enemy.Name == "Trešais")
				{
					unsafeEnemies.Remove(enemy);
				}
			}
		}
		catch (InvalidOperationException)
		{
			GD.Print("Tieša dzēšana foreach ciklā izraisa InvalidOperationException.");
		}

		List<Enemy> enemiesToRemove = new();
		foreach (Enemy enemy in enemies)
		{
			if (enemy.Name == "Trešais")
			{
				enemiesToRemove.Add(enemy);
			}
		}

		foreach (Enemy enemy in enemiesToRemove)
		{
			enemies.Remove(enemy);
		}

		GD.Print($"Ienaidnieku skaits: {enemies.Count}");

		foreach (Enemy enemy in enemies)
		{
			GD.Print(enemy.Name);
		}
	}
}
