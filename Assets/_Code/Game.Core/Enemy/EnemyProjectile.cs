using UnityEngine;

public class EnemyProjectile: MonoBehaviour {
	[SerializeField] private float projectileSpeed;
	[SerializeField] private int projectileDamage;
	private SpriteRenderer enemyProjectileSR;

	void Awake() {
		enemyProjectileSR = GetComponentInChildren<SpriteRenderer>();
		enemyProjectileSR.transform.rotation = Quaternion.identity;
	}

	void Update() {
		transform.position += transform.right * projectileSpeed * Time.deltaTime;
	}

	void OnTriggerEnter2D(Collider2D col) {
		GameObject collidedWith = col.gameObject;

		switch (collidedWith.tag) {
			case "player": {
				collidedWith.GetComponent<PlayerHealth>().DealDamage(projectileDamage, transform.position);
				Destroy(gameObject);
				break;
			}
			case "wall": {
				Destroy(gameObject);
				break;
			}
			default: break;
		}
	}
}
