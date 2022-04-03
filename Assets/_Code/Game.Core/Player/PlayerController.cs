using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core
{
	public class PlayerController : MonoBehaviour
	{
		private Rigidbody2D playerRB;
		private SpriteRenderer playerSR;
		private Animator playerAnimator;
		[SerializeField] private CapsuleCollider2D playerCollider;
		public PlayerHealth Health;

		[SerializeField] private float xMoveSpeed;
		[SerializeField] private float yMoveSpeed;
		[SerializeField] private float diagonalXMoveSpeed;
		[SerializeField] private float xDashSpeed;
		[SerializeField] private float yDashSpeed;
		[SerializeField] private float dashCooldown;
		[SerializeField] private float dashDuration;

		private float dashCounter;
		private bool isDashing;
		private Vector2 rawMovementInput;

		private bool facingRight = true;

		private string currentAnimState;
		private string animIdle = "vampire_animations_Idle";
		private string animDash = "vampire_animations_Dash";
		private string animWalk = "vampire_animations_Walk";

		void Awake()
		{
			playerRB = GetComponent<Rigidbody2D>();
			playerSR = GetComponentInChildren<SpriteRenderer>();
			playerAnimator = GetComponentInChildren<Animator>();
			Health = GetComponent<PlayerHealth>();
			ChangeAnimationState(animIdle);
		}

		void OnEnable()
		{
			GameManager.Game.Controls.Gameplay.Dash.performed += GetDashInput;
		}

		void OnDisable()
		{
			GameManager.Game.Controls.Gameplay.Dash.performed -= GetDashInput;
		}

		void Update()
		{
			if (isDashing)
			{
				playerCollider.enabled = false;
				if (dashCooldown - dashCounter > dashDuration)
				{
					isDashing = false;
				}
			}
			else
			{
				playerCollider.enabled = true;
				rawMovementInput = GameManager.Game.Controls.Gameplay.Move.ReadValue<Vector2>();
			}

			if (dashCounter > 0)
			{
				dashCounter -= Time.deltaTime;
			}

			playerSR.flipX = !facingRight;
		}

		void FixedUpdate()
		{
			if (isDashing)
			{
				ChangeAnimationState(animDash);
				playerRB.velocity = new Vector2(rawMovementInput.x * xDashSpeed, rawMovementInput.y * yDashSpeed);
			}
			else if(Health.getKnockbackCounter() <= 0)
			{
				handleMovement(rawMovementInput);
			}
		}

		private void handleMovement(Vector2 RawMovementInput)
		{
			if (RawMovementInput != Vector2.zero)
			{
				ChangeAnimationState(animWalk);

				if (RawMovementInput.y == 0)
				{
					playerRB.velocity = new Vector2(RawMovementInput.x * xMoveSpeed, 0);
				}
				else
				{
					playerRB.velocity = new Vector2(RawMovementInput.x * diagonalXMoveSpeed, RawMovementInput.y * yMoveSpeed);
				}

				if (playerRB.velocity.x < 0)
				{
					facingRight = false;
				}
				else
				{
					facingRight = true;
				}
			}
			else
			{
				ChangeAnimationState(animIdle);
				playerRB.velocity = Vector2.zero;
			}
		}

		private void GetDashInput(InputAction.CallbackContext context)
		{
			if (context.action.triggered && dashCounter <= 0 && Health.getKnockbackCounter() <= 0)
			{
				isDashing = true;
				dashCounter = dashCooldown;
			}
		}

		void ChangeAnimationState(string newState) {
			if (currentAnimState == newState) return;

			playerAnimator.Play(newState);

			currentAnimState = newState;
		}

		void OnCollisionEnter2D(Collision2D col)
		{
			GameObject collidedWith = col.gameObject;

			if (collidedWith.tag == "enemy")
			{
				EnemyHealth enemyHealth = collidedWith.GetComponent<EnemyHealth>();

				if (isDashing)
				{
					Health.Heal(enemyHealth.GetDrainHealthToPlayer());
				}
				else
				{
					if(!enemyHealth.getDead()) {
						Health.DealDamage(enemyHealth.GetDamageToPlayer(), collidedWith.transform.position);
					}
				}
			}
		}
	}
}

