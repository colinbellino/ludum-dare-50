using Cysharp.Threading.Tasks;
using UnityEditor;

namespace Game.Core.StateMachines.Game
{
	public class GameQuitState : IState
	{
		public GameFSM FSM;

		public UniTask Enter()
		{
			// FIXME: What if this is a WebGL build?
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#else
			UnityEngine.Application.Quit();
#endif

			return default;
		}

		public void Tick() { }

		public void FixedTick() { }

		public UniTask Exit()
		{
			return default;
		}
	}
}
