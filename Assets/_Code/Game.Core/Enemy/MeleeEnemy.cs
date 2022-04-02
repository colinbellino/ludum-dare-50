using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
   private SpriteRenderer enemySR;
   private Rigidbody2D enemyRB;
   private Animator enemyAnimator;
   private EnemyHealth enemyHealth;
   private PlayerHealth playerHealth;

   [SerializeField] private float moveSpeed;
   private Vector2 movement;

   void Awake() {
      enemySR = FindObjectOfType<SpriteRenderer>();
      enemyRB = FindObjectOfType<Rigidbody2D>();
      enemyAnimator = FindObjectOfType<Animator>();
      enemyHealth = FindObjectOfType<EnemyHealth>();
      playerHealth = FindObjectOfType<PlayerHealth>();
   }

   void Update() {
      if (!enemyHealth.getDead() && !playerHealth.getDead()) {
         Vector3 direction = playerHealth.gameObject.transform.position - transform.position;
         direction.Normalize();
         movement = direction;

         if (playerHealth.gameObject.transform.position.x < transform.position.x) {
            Vector3 localScaleTemp = transform.localScale;
            localScaleTemp.x = -1;
            transform.localScale = localScaleTemp;
         }  else {
            Vector3 localScaleTemp = transform.localScale;
            localScaleTemp.x = 1;
            transform.localScale = localScaleTemp;
         }
      }
   }

   void FixedUpdate() {
      if (!enemyHealth.getDead() && !playerHealth.getDead()) {
         attackPlayer(movement);
      }
   }

   public void attackPlayer(Vector2 playerDirection)
    {
        enemyRB.MovePosition((Vector2)transform.position + (playerDirection * moveSpeed * Time.deltaTime));
    }
}