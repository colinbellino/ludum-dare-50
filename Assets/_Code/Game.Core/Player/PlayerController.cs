using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core {
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D playerRB;
        private SpriteRenderer playerSR;
        private Animator playerAnimator;

        [SerializeField] private float xMoveSpeed;
        [SerializeField] private float yMoveSpeed;
        [SerializeField] private float diagonalXMoveSpeed;
        [SerializeField] private float xDashSpeed;
        [SerializeField] private float yDashSpeed;
        [SerializeField] private float dashCooldown;
        [SerializeField] private float dashDuration;

         private float dashCounter;
        private Vector2 RawMovementInput;
        private bool isDashing;

        void Awake()
        {
            playerRB = GetComponent<Rigidbody2D>();
            playerSR = GetComponent<SpriteRenderer>();
            playerAnimator = GetComponent<Animator>();
        }

        void Update() {
            if (!isDashing) {
                RawMovementInput = GameManager.Game.Controls.Gameplay.Move.ReadValue<Vector2>();
            }
           
            GameManager.Game.Controls.Gameplay.Dash.performed += GetDashInput;

            if (isDashing) {
                if (dashCooldown - dashCounter > dashDuration) {
                    isDashing = false;
                }
            }

            if (dashCounter > 0) {
                dashCounter -= Time.deltaTime;
            }
        }

        void FixedUpdate()
        {
            handleMovement(RawMovementInput);   

            if (isDashing) {
                playerRB.velocity = new Vector2(RawMovementInput.x * xDashSpeed, RawMovementInput.y * yDashSpeed);
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
                    playerSR.flipX = true;
                } else {
                    playerSR.flipX = false;
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
    }
}

