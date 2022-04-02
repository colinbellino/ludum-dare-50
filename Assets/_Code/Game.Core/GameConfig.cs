using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Core
{
	[CreateAssetMenu(menuName = "Game/Game Config")]
	public class GameConfig : ScriptableObject
	{
		[Header("DEBUG")]
		public bool DebugStateMachine;
		public bool DebugLevels;
		public int LockFPS = 60;

		[Header("CONTENT")]
		public Level[] Levels;
		public TileToEntity TileToEntity;

		[Header("AUDIO")]
		public string GameBus = "bus:/Game";
		public string MusicBus = "bus:/Game/Music";
		public string SoundBus = "bus:/Game/SFX";
		public EventReference SnapshotPause;
		public EventReference SoundMenuConfirm;
		public EventReference MusicTitle;
		public EventReference MusicMain;

		public static Vector2 ROOM_SIZE = new Vector2(16, 9);
	}

	[Serializable]
	public class TileToEntity : SerializableDictionary<TileBase, Entity> { }

	[Serializable]
	public class TileToInfo : SerializableDictionary<TileBase, TileInfo> { }

	[Serializable]
	public class TileInfo
	{
		public bool Walkable;
	}
}
