using UnityEngine;

public class EnemyHealth : Game.Core.Health
{
   [SerializeField] private int drainHealthToPlayerAmt;
   [SerializeField] private int damageToPlayer;
	[SerializeField] private SpriteRenderer pitchforkSprite;
	private int drainHealthToPlayer;

   protected override void Update() {
      base.Update();
   }

   protected override void Death() {
      base.Death();

		enterStunState();
   }

	public override void DealDamage(int damageDone, Vector3 damageSourceDirection) {
		setCurrentHP(currentHP - damageDone);

		if (currentHP > 0) {
			knockbackDirection = (transform.position - damageSourceDirection).normalized;
			knockbackCounter = knockBackDuration;
		}
	}

   public int GetDrainHealthToPlayer() {
      return drainHealthToPlayer;
   }
   public int GetDamageToPlayer() {
      return damageToPlayer;
   }

	private void enterStunState() {
		entityAnimator.SetBool("isDead", true);
		pitchforkSprite.gameObject.SetActive(false);
		damageToPlayer = 0;
		drainHealthToPlayer = drainHealthToPlayerAmt;
	}
}

