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

        void Awake()
        {
            playerRB = GetComponent<Rigidbody2D>();
            playerSR = GetComponent<SpriteRenderer>();
            playerAnimator = GetComponent<Animator>();
        }

        void FixedUpdate()
        {
            Vector2 RawMovementInput = GameManager.Game.Controls.Gameplay.Move.ReadValue<Vector2>();
            Debug.Log(RawMovementInput);
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
    }
}

