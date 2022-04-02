using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Mathematics;

namespace Game.Core
{
	public static class LevelHelpers
	{
		public static Room GetStartRoom(Level level)
		{
			foreach (var room in level.Rooms)
			{
				if (room.Name == "2")
					return room;
			}

			return null;
		}

		public static Room GetRoomInDirection(Vector2 direction, Level level)
		{
			var x = 0;
			var y = 0;
			if (math.round(direction.x) > 0)
				x = 1;
			else if (math.round(direction.x) < 0)
				x = -1;
			if (math.round(direction.y) > 0)
				y = 1;
			else if (math.round(direction.y) < 0)
				y = -1;

			var positionX = level.CurrentRoom.X + x;
			var positionY = level.CurrentRoom.Y + y;
			var room = GetRoomAtPosition(positionX, positionY, level);

			return room;
		}

		public static Vector3 GetRoomCenter(Room room)
		{
			return new Vector3(room.X * GameConfig.ROOM_SIZE.x + GameConfig.ROOM_SIZE.x / 2, -room.Y * GameConfig.ROOM_SIZE.y + GameConfig.ROOM_SIZE.y / 2);
		}

		public static Vector3 GetRoomOrigin(Room room)
		{
			return new Vector3(room.X * GameConfig.ROOM_SIZE.x, -room.Y * GameConfig.ROOM_SIZE.y);
		}

		public static Room GetRoomAtPosition(int x, int y, Level level)
		{
			foreach (var room in level.Rooms)
			{
				if (room.X == x && room.Y == y && room.Instance != null)
					return room;
			}

			return null;
		}

		public static void TransitionToRoom(Level level, Room roomToTransitionTo)
		{
			DeactivateRoom(level.CurrentRoom);
			ActivateRoom(roomToTransitionTo);

			level.CurrentRoom = roomToTransitionTo;
		}

		public static void ActivateRoom(Room roomToActivate)
		{
			// TODO: close door, spawn entities, etc
			UnityEngine.Debug.Log("Activating room: " + roomToActivate);
		}

		public static void DeactivateRoom(Room roomToDeactivate)
		{
			UnityEngine.Debug.Log("Deactivating room: " + roomToDeactivate);
		}
	}

	public class Level
	{
		public List<Room> Rooms;
		public Room StartRoom;
		public Room CurrentRoom;
	}

	public class Room
	{
		public string Name;
		public int Index;
		public int X;
		public int Y;
		public GameObject Instance;

		public override string ToString()
		{
			return $"Room [{X},{Y}]: {Name}";
		}
	}
}
