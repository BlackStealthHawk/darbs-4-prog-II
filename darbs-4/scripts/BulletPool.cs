using Godot;
using System.Collections.Generic;

public class BulletPool
{
	private readonly List<Bullet> pool = new();

	public void Initialize(Node parent, PackedScene bulletScene, int poolSize = 50)
	{
		if (parent == null || bulletScene == null || pool.Count > 0) return;

		for (int i = 0; i < poolSize; i++)
		{
			Bullet bullet = bulletScene.Instantiate<Bullet>();
			parent.AddChild(bullet);
			bullet.Deactivate();
			pool.Add(bullet);
		}

		GD.Print($"[BulletPool] Sagatavoti {pool.Count} rezerves lādiņi.");
	}

	public Bullet GetBullet()
	{
		foreach (Bullet bullet in pool)
		{
			if (!bullet.IsActive)
			{
				return bullet;
			}
		}

		return null;
	}

	public void ReturnBullet(Bullet bullet)
	{
		bullet?.Deactivate();
	}
}
