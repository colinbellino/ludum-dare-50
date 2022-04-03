using UnityEngine;

public class BatProjectile: MonoBehaviour {
	[SerializeField] private float projectileSpeed;
	private SpriteRenderer batSR;

	void Awake() {
		batSR = GetComponentInChildren<SpriteRenderer>();
		batSR.transform.rotation = Quaternion.identity;
	}

	void Update() {
		transform.position += transform.right * projectileSpeed * Time.deltaTime;
	}

	void OnTriggerEnter2D(Collider2D col) {
		GameObject collidedWith = col.gameObject;

		switch (collidedWith.tag) {
			case "enemy": {
				if (!collidedWith.GetComponent<EnemyHealth>().getDead()) {
					collidedWith.GetComponent<EnemyHealth>().DealDamage(1, transform.position);
					Destroy(gameObject);
				}
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
