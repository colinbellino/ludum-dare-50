using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

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

		public static async UniTask<string> ReadCommitFromFile()
		{
			var path = Application.streamingAssetsPath + "/commit.txt";

			if (IsWebGL())
				return (await UnityWebRequest.Get(path).SendWebRequest()).downloadHandler.text;

			return File.ReadAllText(path);
		}

		public static Vector3 SnapTo(Vector3 v3, float snapAngle)
		{
			float angle = Vector3.Angle(v3, Vector3.up);
			if (angle < snapAngle / 2.0f)          // Cannot do cross product
				return Vector3.up * v3.magnitude;  //   with angles 0 & 180
			if (angle > 180.0f - snapAngle / 2.0f)
				return Vector3.down * v3.magnitude;

			float t = Mathf.Round(angle / snapAngle);
			float deltaAngle = (t * snapAngle) - angle;

			Vector3 axis = Vector3.Cross(Vector3.up, v3);
			Quaternion q = Quaternion.AngleAxis(deltaAngle, axis);
			return q * v3;
		}
	}
}
