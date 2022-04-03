using UnityEngine;

public class PlayerHealth : Game.Core.Health
{
	[SerializeField] private int healthLossPerSecond;
	[SerializeField] private float healthLossCooldown;
	private float timer;

	[SerializeField] private float invicibilityDuration;
	private float invincibilityCounter;

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

		if (invincibilityCounter > 0) {
			invincibilityCounter -= Time.deltaTime;

			if (invincibilityCounter <= 0) {
				entitySR.color = new Color(entitySR.color.r, entitySR.color.g, entitySR.color.b, 1);
			}
		}
	}

	public override void DealDamage(int damageDone, Vector3 damageSourceDirection) {
		if (invincibilityCounter <= 0) {
			if (currentHP - damageDone < 0) {
				setCurrentHP(0);
			} else {
				setCurrentHP(currentHP - damageDone);
			}

			if (currentHP > 0) {
				if (damageSourceDirection.magnitude > 0) {
					knockbackDirection = (transform.position - damageSourceDirection).normalized;
					knockbackCounter = knockBackDuration;
				}

				invincibilityCounter = invicibilityDuration;
				entitySR.color = new Color(entitySR.color.r, entitySR.color.g, entitySR.color.b, 0.5f);
			}
		}
	}

	public void Heal(int healAmount)
	{
		if (currentHP + healAmount > maxHP) {
			setCurrentHP(maxHP);
		} else {
			setCurrentHP(currentHP + healAmount);
		}
	}
}

