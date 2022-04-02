using UnityEngine;
using System.Collections.Generic;
using System;

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
			return null;
		}
	}

	public struct Level
	{
		public List<Room> Rooms;
		public Room Start;
		public Room Current;
	}

	public class Room
	{
		public string Name;
		public int Index;
		public int X;
		public int Y;
		public GameObject Instance;
	}
}
