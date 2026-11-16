using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyListDemo : Node
{
	private List<string> enemies = new()
	{
		"Pirmais",
		"Otrais",
		"Trešais",
		"Ceturtais"
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

		List<string> unsafeEnemies = new(enemies);
		try
		{
			foreach (string enemy in unsafeEnemies)
			{
				if (enemy == "Trešais")
				{
					unsafeEnemies.Remove(enemy);
				}
			}
		}
		catch (InvalidOperationException)
		{
			GD.Print("Tieša dzēšana foreach ciklā izraisa InvalidOperationException.");
		}

		List<string> enemiesToRemove = new();
		foreach (string enemy in enemies)
		{
			if (enemy == "Trešais")
			{
				enemiesToRemove.Add(enemy);
			}
		}

		foreach (string enemy in enemiesToRemove)
		{
			enemies.Remove(enemy);
		}

		GD.Print($"Ienaidnieku skaits: {enemies.Count}");

		foreach (string enemy in enemies)
		{
			GD.Print(enemy);
		}
	}
}
