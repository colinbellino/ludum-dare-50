using System;
using System.Collections.Generic;
using FMOD.Studio;

namespace Game.Core
{
	public class GameState
	{
		public string Version;
		public string Commit;

		public Unity.Mathematics.Random Random;
		public bool Running;
		public bool Paused;
		public float TimeScaleCurrent;
		public float TimeScaleDefault;
		public Level[] DebugLevels;
		public Level[] AllLevels;

		public EventInstance TitleMusic;
		public EventInstance LevelMusic;
		public EventInstance PauseSnapshot;

		public int CurrentLevelIndex;
		public Level Level;
		public List<Entity> Entities = new List<Entity>(30);

		public PlayerSettings PlayerSettings;
		public PlayerSaveData PlayerSaveData;
	}

	[Serializable]
	public struct PlayerSettings
	{
		public string LocaleCode;

		public float GameVolume;
		public float SoundVolume;
		public float MusicVolume;

		public bool FullScreen;
		public int ResolutionWidth;
		public int ResolutionHeight;
		public int ResolutionRefreshRate;
	}

	[Serializable]
	public struct PlayerSaveData
	{
		public HashSet<int> ClearedLevels;
	}
}
