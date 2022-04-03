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

