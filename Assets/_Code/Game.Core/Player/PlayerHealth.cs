using UnityEngine;

public class PlayerHealth : Game.Core.Health
{
	[SerializeField] private int healthLossPerSecond;
	[SerializeField] private float healthLossCooldown;
	private float timer;

	protected override void Death()
	{
		base.Death();

		// change game state
		// play death animation
	}

	protected override void Update()
	{
		base.Update();

		if (currentHP > 0)
		{
			timer += Time.deltaTime;
			if (timer >= healthLossCooldown)
			{
				timer = 0f;
				setCurrentHP(currentHP - healthLossPerSecond);
			}
		}
	}

	public void Heal(int healAmount)
	{
		setCurrentHP(currentHP + healAmount);
	}
}

