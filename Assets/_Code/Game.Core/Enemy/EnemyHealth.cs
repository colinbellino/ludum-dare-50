using UnityEngine;

public class EnemyHealth : Game.Core.Health
{
   [SerializeField] private int drainHealthToPlayer;
   [SerializeField] private int damageToPlayer;
	[SerializeField] private SpriteRenderer pitchforkSprite;

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
	}
}

