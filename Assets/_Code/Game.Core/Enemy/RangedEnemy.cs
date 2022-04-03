using UnityEngine;
using Game.Core;

public class RangedEnemy : MonoBehaviour {
	private EnemyHealth enemyHealth;
	private PlayerHealth playerHealth;
	[SerializeField] private Transform crossTransform;
	[SerializeField] private SpriteRenderer enemyBodySR;
   private Rigidbody2D enemyRB;
   private Animator enemyAnimator;
	private Entity enemyEntity;
	[SerializeField] private GameObject enemyProjectile;

	private Vector2 directionToPlayer;
	private float shootCounter;
	[SerializeField] private float shootCooldown;

	void Awake()
	{
		enemyRB = GetComponent<Rigidbody2D>();
		enemyAnimator = GetComponentInChildren<Animator>();
		enemyHealth = GetComponent<EnemyHealth>();
		enemyEntity = GetComponent<Entity>();
	}

	void OnEnable()
	{
		if (enemyEntity.Ready)
		{
			playerHealth = GameManager.Game.State.Player.GetComponent<PlayerHealth>();
		}
	}

   void Update() {
      if (!enemyHealth.getDead() && !playerHealth.getDead()) {
         if (playerHealth.gameObject.transform.position.x < transform.position.x) {
            enemyBodySR.flipX = false;
         }  else {
            enemyBodySR.flipX = true;
         }

			if (shootCounter > 0) {
				shootCounter -=Time.deltaTime;
			}
      }
   }

   void FixedUpdate() {
      if (!enemyHealth.getDead() && !playerHealth.getDead()) {
			if (shootCounter <= 0) {
				attackPlayer(directionToPlayer);
			}

 			float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
			crossTransform.localRotation = Quaternion.Euler(0, 0, angle-180);
      }
	}

	private void attackPlayer(Vector2 playerDirection) {
		GameObject projectile = Instantiate(enemyProjectile, transform.position, transform.rotation);
		shootCounter = shootCooldown;
	}
}

