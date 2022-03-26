using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Core
{
	public static class Utils
	{
		public static bool IsDevBuild()
		{
#if UNITY_EDITOR
			return true;
#endif

#pragma warning disable 162
			if (Debug.isDebugBuild)
			{
				return true;
			}

			return false;
#pragma warning restore 162
		}

		public static UniTask WaitForCurrentAnimation(Entity entity, float timeScale)
		{
			var infos = entity.Animator.GetCurrentAnimatorClipInfo(0);
			if (infos.Length == 0)
				return default;

			var clipName = entity.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
			if (entity.AnimationClipLength.ContainsKey(clipName) == false)
				return default;

			var length = entity.AnimationClipLength[clipName];
			return UniTask.Delay(System.TimeSpan.FromSeconds(length / timeScale));
		}

		public static bool IsWebGL()
		{
			return Application.platform == RuntimePlatform.WebGLPlayer;
		}

		public static bool IsTileWalkable(Tile tile)
		{
			if (tile.colliderType == Tile.ColliderType.None)
				return false;

			return true;
		}
	}
}
