using UnityEngine;
using UnityEngine.InputSystem
using Game.Core;

public class PlayerProjectile : MonoBehaviour
{
	private PlayerHealth playerHealth;
	[SerializeField] private GameObject batProjectile;

	[SerializeField] private float shootCooldown;
	private float shootCounter;

    void Awake()
    {
		playerHealth = GameManager.Game.State.Player.GetComponent<PlayerHealth>();
    }

	void OnEnable() {
		GameManager.Game.Controls.Gameplay.Shoot.performed += GetShootInput;
	}
	void OnDisable() {
		GameManager.Game.Controls.Gameplay.Shoot.performed -= GetShootInput;
	}

    void Update()
    {
		if (!playerHealth.getDead()) {

		}
    }

	private void GetShootInput(InputAction.CallbackContext context) {
		if (context.action.triggered && shootCounter <= 0)
			{
				Instantiate(batProjectile, gameObject.transform.position, Quaternion.identity);
			}
	}
}
