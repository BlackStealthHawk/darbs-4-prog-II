using Godot;
using System.Collections.Generic;
using System;

public partial class Inventory : Node
{
	private Dictionary<string, int> inventory = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		inventory["Health Potion"] = 5;
		inventory["Mana Potion"] = 3;
		inventory["Ammo Pack"] = 20;

		GD.Print("Health Potion: ", GetAmount("Health Potion"));

		if (GetAmount("Health Potion") > 0)
		{
			GD.Print("Health Potion ir pieejams inventārā.");
		}
		else
		{
			GD.Print("Health Potion nav inventārā.");
		}

		foreach (var pair in inventory)
		{
			GD.Print(pair.Key, ": ", pair.Value);
		}

		if (inventory.TryGetValue("Health Potion", out int amount))
		{
			GD.Print("Atrasts: ", amount);
		}

		GD.Print("Ammo Pack: ", GetAmount("Ammo Pack"));
		GD.Print("Shield: ", GetAmount("Shield"));
		GD.Print("Inventory count: ", inventory.Count);
	}

	public int GetAmount(string name)
	{
		if (inventory.TryGetValue(name, out int amount))
		{
			return amount;
		}

		return 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
