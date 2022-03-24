using Cysharp.Threading.Tasks;

namespace Game.Core.StateMachines.Game
{
	public class GameLoadLevelState : IState
	{
		public GameFSM FSM;

		public async UniTask Enter()
		{
			await UniTask.NextFrame();

			FSM.Fire(GameFSM.Triggers.Done);
		}

		public UniTask Exit()
		{
			return default;
		}

		public void FixedTick() { }

		public void Tick() { }
	}
}
