using UnityEngine;

namespace Game.Core
{
	public class Entity : MonoBehaviour
	{
		[SerializeField] public Animator Animator;
		[SerializeField] public SpriteRenderer SpriteRenderer;

		public Vector3 SpawnPosition;
		public bool Ready;

		private void OnEnable()
		{
			if (Ready)
			{
				SpriteRenderer.color = Color.green;
			}
		}

		private void OnDisable()
		{
			if (Ready)
			{
				transform.localPosition = SpawnPosition;
				SpriteRenderer.color = Color.red;
			}
		}
	}
}
