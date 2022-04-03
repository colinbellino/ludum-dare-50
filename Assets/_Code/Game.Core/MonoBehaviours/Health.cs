using System;
using UnityEngine;

namespace Game.Core
{
	public class Health : MonoBehaviour
	{
		[SerializeField] protected int maxHP;
		public int currentHP { get; private set; }
		protected bool dead;

		public Action<int, int> CurrentHPChanged;

		protected void Awake()
		{
			currentHP = maxHP;
		}

		protected virtual void Update()
		{
			if (currentHP <= 0 && !dead)
			{
				Death();
			}
		}

		public void DealDamage(int damageDone)
		{
			currentHP -= damageDone;
		}

		public int getMaxHP()
		{
			return this.maxHP;
		}
		public void setCurrentHP(int value)
		{
			UnityEngine.Debug.Log(name + " set HP: " + value);
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
