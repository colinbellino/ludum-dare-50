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

			var startRoom = GetStartRoom(GameManager.Game.State.Level);
			var startPosition = new Vector3(startRoom.X * GameConfig.ROOM_SIZE.x, -startRoom.Y * GameConfig.ROOM_SIZE.y);
			GameManager.Game.CameraRig.transform.position = startPosition;

			await UniTask.NextFrame();

			FSM.Fire(GameFSM.Triggers.Done);
		}

		public UniTask Exit()
		{
			return default;
		}

		public void FixedTick() { }

		public void Tick() { }

		private static Room GetStartRoom(Level level)
		{
			foreach (var room in level.Rooms)
			{
				if (room.Name == "2")
					return room;
			}

			return null;
		}
	}
}

namespace Game.Core
{
	public struct Level
	{
		public List<Room> Rooms;
	}

	public class Room
	{
		public string Name;
		public int X;
		public int Y;
		public GameObject Instance;
	}
}
