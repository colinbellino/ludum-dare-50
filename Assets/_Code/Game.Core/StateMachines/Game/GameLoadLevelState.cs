using Cysharp.Threading.Tasks;

namespace Game.Core.StateMachines.Game
{
	public class GameLoadLevelState : IState
	{
		public GameFSM FSM;

		public async UniTask Enter()
		{
			var level = LevelLoader.LoadLevel("Level1");
			level.Start = LevelHelpers.GetStartRoom(level);
			level.Current = level.Start;

			GameManager.Game.CameraRig.transform.position = LevelHelpers.GetRoomCenter(level.Current);

			GameManager.Game.State.Level = level;

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
