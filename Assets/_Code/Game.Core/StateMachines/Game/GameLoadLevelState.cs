using Cysharp.Threading.Tasks;
using UnityEngine;

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

			var startPosition = new Vector3(level.Start.X * GameConfig.ROOM_SIZE.x, -level.Start.Y * GameConfig.ROOM_SIZE.y);
			GameManager.Game.CameraRig.transform.position = startPosition;

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
