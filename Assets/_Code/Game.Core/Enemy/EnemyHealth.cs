using UnityEngine;

public class EnemyHealth : Game.Core.Health
{
   [SerializeField] private int drainHealthToPlayer;
   [SerializeField] private int damageToPlayer;

   protected override void Update() {
      base.Update();
   }
   
   protected override void Death() {
      base.Death();

      // play stun animation
      // stop movement and attack
   }

   public int GetDrainHealthToPlayer() {
      return drainHealthToPlayer;
   }
   public int GetDamageToPlayer() {
      return damageToPlayer;
   }
}

