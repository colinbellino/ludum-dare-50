using Cysharp.Threading.Tasks;
using UnityEngine;
using FMOD.Studio;

namespace Game.Core.StateMachines.Game
{
	public class GameCreditsState : IState
	{
		public GameFSM FSM;

		public async UniTask Enter()
		{
			GameManager.Game.UI.SetDebugText("");

			await GameManager.Game.UI.ShowCredits();
			await GameManager.Game.UI.FadeOut();
		}

		public void Tick()
		{
			if (GameManager.Game.Controls.Global.Cancel.WasPerformedThisFrame() || GameManager.Game.Controls.Global.Confirm.WasPerformedThisFrame())
			{
				FSM.Fire(GameFSM.Triggers.Done);
			}
		}

		public void FixedTick() { }

		public async UniTask Exit()
		{
			GameManager.Game.State.LevelMusic.stop(STOP_MODE.ALLOWFADEOUT);
			await GameManager.Game.UI.FadeIn(Color.black);
			await GameManager.Game.UI.HideCredits();
		}
	}
}
