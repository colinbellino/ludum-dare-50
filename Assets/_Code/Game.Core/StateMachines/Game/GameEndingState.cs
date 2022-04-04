using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Core.StateMachines.Game
{
	public class GameEndingState : IState
	{
		public GameFSM FSM;

		public async UniTask Enter()
		{
			await GameManager.Game.UI.ShowEnding(0);
			await GameManager.Game.UI.FadeIn(Color.clear);
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
			await GameManager.Game.UI.FadeIn(Color.black);
			await GameManager.Game.UI.HideEnding(0);
		}
	}
}
