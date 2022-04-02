using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core {
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D playerRB;
        private SpriteRenderer playerSR;
        private Animator playerAnimator;
        private CapsuleCollider2D playerCollider;
        private PlayerHealth playerHealth;

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
        private float colliderOffsetY = -0.05737776f;
        private float colliderOffsetXFacingRight = 0.05f;
        private float colliderOffsetXFacingLeft = -0.08f;
        

        void Awake()
        {
            playerRB = GetComponent<Rigidbody2D>();
            playerSR = GetComponent<SpriteRenderer>();
            playerAnimator = GetComponent<Animator>();
            playerCollider = GetComponent<CapsuleCollider2D>();
            playerHealth = GetComponent<PlayerHealth>();
        }

        void Update() {
            GameManager.Game.Controls.Gameplay.Dash.performed += GetDashInput;

            if (isDashing) {
                if (dashCooldown - dashCounter > dashDuration) {
                    isDashing = false;
                }
            } else {
                rawMovementInput = GameManager.Game.Controls.Gameplay.Move.ReadValue<Vector2>();
            }

            if (dashCounter > 0) {
                dashCounter -= Time.deltaTime;
            }

            if (facingRight) {
                Vector3 localScaleTemp = transform.localScale;
                localScaleTemp.x = 1;
                transform.localScale = localScaleTemp;
            } else {
                Vector3 localScaleTemp = transform.localScale;
                localScaleTemp.x = -1;
                transform.localScale = localScaleTemp;
            }
        }

        void FixedUpdate()
        {
            if (isDashing) {
                playerRB.velocity = new Vector2(rawMovementInput.x * xDashSpeed, rawMovementInput.y * yDashSpeed);
            }  else {
                handleMovement(rawMovementInput);  
            }     
        }

        private void handleMovement(Vector2 RawMovementInput) {
            if (RawMovementInput != Vector2.zero) {
                if (RawMovementInput.y == 0) {
                    playerRB.velocity = new Vector2(RawMovementInput.x * xMoveSpeed, 0);
                } else {
                    playerRB.velocity = new Vector2(RawMovementInput.x * diagonalXMoveSpeed, RawMovementInput.y * yMoveSpeed);
                }

                if (playerRB.velocity.x < 0) {
                    facingRight = false;
                } else {
                    facingRight = true;
                }  
            } else {
                playerRB.velocity = Vector2.zero;
            }
        }

        private void GetDashInput(InputAction.CallbackContext context) {
            if (context.action.triggered && dashCounter <= 0) {
                isDashing = true;
                dashCounter = dashCooldown;
            }
        }

        void OnCollisionEnter2D(Collision2D col) {
            GameObject collidedWith = col.gameObject;

            if (collidedWith.tag == "enemy") {
                EnemyHealth enemyHealth = collidedWith.GetComponent<EnemyHealth>();

                if (isDashing) {
                    playerHealth.Heal(enemyHealth.GetDrainHealthToPlayer());
                } else {
                    playerHealth.DealDamage(enemyHealth.GetDamageToPlayer());
                }
            }
        }
    }
}

