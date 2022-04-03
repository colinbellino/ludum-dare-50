using System;
using UnityEngine;

namespace Game.Core
{
	public class Health : MonoBehaviour
	{
		[SerializeField] protected int maxHP;
		public int currentHP { get; private set; }
		protected bool dead;
		protected Animator entityAnimator;
		private Rigidbody2D entityRB;

		public float knockbackCounter {get; private set;}
		[SerializeField] private float knockbackForce;
		[SerializeField] private float knockBackDuration;
		private Vector2 knockbackDirection;

		public Action<int, int> CurrentHPChanged;

		protected virtual void Awake()
		{
			currentHP = maxHP;
			entityAnimator = GetComponentInChildren<Animator>();
			entityRB = GetComponent<Rigidbody2D>();
		}

		protected virtual void Update()
		{
			if (currentHP <= 0 && !dead)
			{
				Death();
			}

			if (knockbackCounter > 0) {
				knockbackCounter -= Time.deltaTime;
			}
		}

		void FixedUpdate() {
			if (knockbackCounter > 0) {
				// entityRB.velocity = new Vector2(knockbackDirection.x * knockbackForce, knockbackDirection.y * knockbackForce);
				transform.Translate(knockbackDirection * knockbackForce);
			}
		}

		public void DealDamage(int damageDone, Vector3 damageSourceDirection)
		{
			setCurrentHP(currentHP - damageDone);

			if (currentHP > 1) {
				knockbackDirection = (transform.position - damageSourceDirection).normalized;
				knockbackCounter = knockBackDuration;
			}
		}

		public int getMaxHP()
		{
			return this.maxHP;
		}
		public void setCurrentHP(int value)
		{
			this.currentHP = value;
			CurrentHPChanged?.Invoke(currentHP, maxHP);
		}

		protected virtual void Death()
		{
			dead = true;
		}

		public bool getDead()
		{
			return dead;
		}
	}
}
