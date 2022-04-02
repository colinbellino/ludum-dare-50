using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Core
{
	public static class LevelLoader
	{
		public static bool LoadLevelFile(string path, ref string data)
		{
			try
			{
				UnityEngine.Debug.Log("Loading level: " + path);
				data = File.ReadAllText(path);
				return true;
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError(e.Message);
				return false;
			}
		}

		public static Level LoadLevel(string levelName)
		{
			var levelData = "";
			LoadLevelFile($"{Application.streamingAssetsPath}/_Levels/{levelName}.txt", ref levelData);
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

				GameObject roomInstance = null;
				var roomType = int.Parse(character.ToString());

				if (roomType > 0)
				{
					var roomPrefab = Resources.Load<GameObject>("Rooms/Room" + character);
					roomInstance = GameObject.Instantiate(roomPrefab);
					roomInstance.name = $"[{x},{y}] {character}";
					roomInstance.transform.position = new Vector3(x * GameConfig.ROOM_SIZE.x, -y * GameConfig.ROOM_SIZE.y);
				}

				level.Rooms.Add(new Room
				{
					X = x,
					Y = y,
					Index = i,
					Name = character.ToString(),
					Instance = roomInstance,
				});

				x += 1;
				i += 1;
			}

			return level;
		}
	}
}
