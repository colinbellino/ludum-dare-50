using Cysharp.Threading.Tasks;

namespace Game.Core.StateMachines.Game
{
	public class GameLoadLevelState : BaseGameState
	{
		public GameLoadLevelState(GameFSM fsm, GameSingleton game) : base(fsm, game) { }

		public override async UniTask Enter()
		{
			await base.Enter();

			await UniTask.NextFrame();

			_fsm.Fire(GameFSM.Triggers.Done);
		}
	}
}
