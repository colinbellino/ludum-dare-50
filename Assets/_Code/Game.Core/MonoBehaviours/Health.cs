using UnityEngine;

namespace Game.Core {
    public class Health : MonoBehaviour
    {
        [SerializeField] protected int maxHP;
        protected int currentHP;
        protected bool dead;
        
        protected void Awake()
        {
            currentHP = maxHP;
        }

        protected virtual void Update()
        {
            if (currentHP <= 0 && !dead) {
                Death();
            }   
        }

        public void DealDamage(int damageDone) {
            currentHP -= damageDone;
        }

        public int getMaxHP() {
            return this.maxHP;
        }
        public int getCurrentHP() {
            return this.currentHP;
        }

        protected virtual void Death() {
            dead = true;
        }

        public bool getDead() {
            return dead;
        }
    }
}
