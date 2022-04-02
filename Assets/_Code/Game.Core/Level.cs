using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Mathematics;
using System.IO;

namespace Game.Core
{
	public static class LevelHelpers
	{
		private static Vector3Int[] DIRECTIONS = new Vector3Int[] {
			Vector3Int.up,
			Vector3Int.right,
			Vector3Int.down,
			Vector3Int.left,
		};
		private static Vector3Int[] DOOR_POSITIONS = new Vector3Int[] {
			new Vector3Int(7, 8, 0),
			new Vector3Int(14, 4, 0),
			new Vector3Int(7, 0, 0),
			new Vector3Int(0, 4, 0),
		};

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
			// UnityEngine.Debug.Log("Activating room: " + roomToActivate);
			foreach (var entity in roomToActivate.Entities)
			{
				entity.gameObject.SetActive(true);
			}

#if UNITY_EDITOR
			roomToActivate.Instance.name += " CURRENT";
#endif
		}

		public static void DeactivateRoom(Room roomToDeactivate)
		{
			// UnityEngine.Debug.Log("Deactivating room: " + roomToDeactivate);
			foreach (var entity in roomToDeactivate.Entities)
			{
				entity.gameObject.SetActive(false);
			}

#if UNITY_EDITOR
			roomToDeactivate.Instance.name = roomToDeactivate.Instance.name.Replace(" CURRENT", "");
#endif
		}

		public static string LoadLevelDataFromFile(string path)
		{
			try
			{
				UnityEngine.Debug.Log("Loading level: " + path);
				return File.ReadAllText(path);
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError(e.Message);
				return "";
			}
		}

		public static Level InstantiateLevel(string levelData)
		{
			levelData = levelData.Trim();

			var level = new Level
			{
				Rooms = new List<Room>(),
			};

			var x = 0;
			var y = 0;
			var i = 0;
			foreach (var character in levelData)
			{
				if (character == '\n')
				{
					x = 0;
					y += 1;
					continue;
				}

				RoomBehaviour roomInstance = null;
				var entities = new List<Entity>();

				var roomType = int.Parse(character.ToString());
				if (roomType > 0)
				{
					var roomPrefab = Resources.Load<RoomBehaviour>("Rooms/Room" + character);
					roomInstance = GameObject.Instantiate(roomPrefab);
					roomInstance.transform.position = new Vector3(x * GameConfig.ROOM_SIZE.x, -y * GameConfig.ROOM_SIZE.y);
#if UNITY_EDITOR
					roomInstance.name = $"[{x},{y}] {character}";
#endif

					foreach (Transform child in roomInstance.transform)
					{
						var entity = child.gameObject.GetComponent<Entity>();
						if (entity == null)
							continue;

						entity.SpawnPosition = entity.transform.localPosition;
						entity.Ready = true;
						entity.gameObject.SetActive(false);
						entities.Add(entity);
					}
				}

				var room = new Room
				{
					X = x,
					Y = y,
					Index = i,
					Name = character.ToString(),
					Instance = roomInstance,
					Entities = entities,
				};
				level.Rooms.Add(room);

				x += 1;
				i += 1;
			}

			// Do a second loop once we have all the rooms to check for connections for doors.
			foreach (var room in level.Rooms)
			{
				if (room.Instance == null)
					continue;

				for (int directionIndex = 0; directionIndex < DIRECTIONS.Length; directionIndex++)
				{
					var direction = DIRECTIONS[directionIndex];
					direction.y = -direction.y;
					var nextRoom = GetRoomAtPosition(room.X + direction.x, room.Y + direction.y, level);
					if (nextRoom == null)
					{
						room.Instance.WallsTilemap.SetTile(DOOR_POSITIONS[directionIndex], GameManager.Game.Config.WallTiles[directionIndex]);
					}
				}
			}

			return level;
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
		public RoomBehaviour Instance;
		public List<Entity> Entities;

		public override string ToString()
		{
			return $"Room [{X},{Y}]: {Name}";
		}
	}
}
