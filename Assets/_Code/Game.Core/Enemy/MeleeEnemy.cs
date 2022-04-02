using UnityEngine;
using Game.Core;

public class MeleeEnemy : MonoBehaviour
{
	private SpriteRenderer enemySR;
	private Rigidbody2D enemyRB;
	private Animator enemyAnimator;
	private EnemyHealth enemyHealth;
	private PlayerHealth playerHealth;
	private Entity enemyEntity;

	[SerializeField] private float moveSpeed;
	private Vector2 directionToPlayer;

	void Awake()
	{
		enemySR = GetComponentInChildren<SpriteRenderer>();
		enemyRB = GetComponent<Rigidbody2D>();
		enemyAnimator = GetComponent<Animator>();
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

	void Update()
	{
		if (!enemyHealth.getDead() && !playerHealth.getDead())
		{
			Vector3 direction = playerHealth.gameObject.transform.position - transform.position;
			direction.Normalize();
			directionToPlayer = direction;

			if (playerHealth.gameObject.transform.position.x < transform.position.x)
			{
				Vector3 localScaleTemp = transform.localScale;
				localScaleTemp.x = -1;
				transform.localScale = localScaleTemp;
			}
			else
			{
				Vector3 localScaleTemp = transform.localScale;
				localScaleTemp.x = 1;
				transform.localScale = localScaleTemp;
			}
		}
	}

	void FixedUpdate()
	{
		if (!enemyHealth.getDead() && !playerHealth.getDead())
		{
			attackPlayer(directionToPlayer);
		}
		else
		{
			enemyRB.velocity = Vector2.zero;
		}
	}

	private void attackPlayer(Vector2 playerDirection)
	{
		enemyRB.MovePosition((Vector2)transform.position + (playerDirection * moveSpeed * Time.deltaTime));
	}
}
