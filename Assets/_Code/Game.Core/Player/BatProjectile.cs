using UnityEngine;

public class BatProjectile: MonoBehaviour {
	[SerializeField] private float projectileSpeed;
	private SpriteRenderer batSR;

	void Awake() {
		batSR = GetComponentInChildren<SpriteRenderer>();
		batSR.transform.rotation = Quaternion.identity;
	}

	void Update() {
		transform.Translate(transform.position * projectileSpeed * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D col) {
		GameObject collidedWith = col.gameObject;

		switch (collidedWith.tag) {
			case "enemy": {
				collidedWith.GetComponent<EnemyHealth>().DealDamage(1);
				DestroyImmediate(this);
				break;
			}
			case "wall": {
				DestroyImmediate(this);
				break;
			}
		}
	}
}
