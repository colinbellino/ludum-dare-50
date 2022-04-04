using UnityEngine;
using Game.Core;

public class PlayerHealth : Game.Core.Health
{
	[SerializeField] private int healthLossPerSecond;
	[SerializeField] private float healthLossCooldown;
	private float timer;

	[SerializeField] private float invicibilityDuration;
	private float invincibilityCounter;
	private float redColorValue;

	[SerializeField] float heartbeatSfxDuration;

	protected override void Awake() {
		base.Awake();

		redColorValue = entitySR.color.r;
		InvokeRepeating("CallHeartbeat", 0, heartbeatSfxDuration);
	}

	protected override void Death()
	{
		base.Death();
		AudioHelpers.PlayOneShot(GameManager.Game.Config.PlayerDeath);
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
				AudioHelpers.PlayOneShot(GameManager.Game.Config.PlayerDamage);

				if (damageSourceDirection.magnitude > 0) {
					knockbackDirection = (transform.position - damageSourceDirection).normalized;
					knockbackCounter = knockBackDuration;
					entitySR.color = new Color(1, 0, 0, 1);
				}

				invincibilityCounter = invicibilityDuration;
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

	private void CallHeartbeat() {
		if ((float)currentHP / (float)maxHP < 0.3) {
			AudioHelpers.PlayOneShot(GameManager.Game.Config.HeartBeat, transform.position);
		}
	}
}

