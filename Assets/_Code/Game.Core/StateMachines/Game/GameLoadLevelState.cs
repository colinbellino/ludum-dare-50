using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Core.StateMachines.Game
{
	public class GameLoadLevelState : IState
	{
		public GameFSM FSM;

		public async UniTask Enter()
		{
			GameManager.Game.State.Level = LevelLoader.LoadLevel("Level0");

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

namespace Game.Core
{
	public struct Level
	{
		public List<Room> Rooms;
	}

	public struct Room
	{
		public string Name;
		public int X;
		public int Y;
		public GameObject Instance;
	}
}
