using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Game.Core
{
	public class Save
	{
		private string _playerSettingsPath = Application.persistentDataPath + "/Settings.bin";
		private string _playerSettingsKey = "PlayerSettings";
		private string _playerSaveDataPath = Application.persistentDataPath + "/Save0.bin";
		private string _playerDataKey = "PlayerSave0";

		public PlayerSettings LoadPlayerSettings()
		{
			var data = new PlayerSettings
			{
				GameVolume = 1,
				SoundVolume = 1,
				MusicVolume = 1,
				FullScreen = Screen.fullScreen,
				ResolutionWidth = Screen.currentResolution.width,
				ResolutionHeight = Screen.currentResolution.height,
				ResolutionRefreshRate = Screen.currentResolution.refreshRate,
				LocaleCode = LocalizationSettings.SelectedLocale.Identifier.Code,
			};

			if (Utils.IsWebGL())
			{
				UnityEngine.Debug.Log("Loading player settings: " + _playerSettingsKey);
				if (PlayerPrefsSerializer.Deserialize(_playerSettingsKey, ref data) == false)
					UnityEngine.Debug.LogWarning("Couldn't load player settings.");
			}
			else
			{
				UnityEngine.Debug.Log("Loading player settings: " + _playerSettingsPath);
				if (BinaryFileSerializer.Deserialize(_playerSettingsPath, ref data) == false)
					UnityEngine.Debug.LogWarning("Couldn't load player settings.");
			}

			return data;
		}

		public void SavePlayerSettings(PlayerSettings data)
		{
			if (Utils.IsWebGL())
			{
				PlayerPrefsSerializer.Serialize(data, _playerSettingsKey);
				UnityEngine.Debug.Log("Saving player settings: " + _playerSettingsKey);
			}
			else
			{
				BinaryFileSerializer.Serialize(data, _playerSettingsPath);
				UnityEngine.Debug.Log("Saving player settings: " + _playerSettingsPath);
			}
		}

		public PlayerSaveData LoadPlayerSaveData()
		{
			var data = new PlayerSaveData
			{
				ClearedLevels = new HashSet<int>(),
			};

			if (Utils.IsWebGL())
			{
				UnityEngine.Debug.Log("Loading player data: " + _playerDataKey);
				if (PlayerPrefsSerializer.Deserialize(_playerDataKey, ref data) == false)
					UnityEngine.Debug.LogWarning("Couldn't load player data.");
			}
			else
			{
				if (BinaryFileSerializer.Deserialize(_playerSaveDataPath, ref data) == false)
					UnityEngine.Debug.LogWarning("Couldn't load player data.");
			}

			return data;
		}

		public void SavePlayerSaveData(PlayerSaveData data)
		{
			if (Utils.IsWebGL())
			{
				BinaryFileSerializer.Serialize(data, _playerSaveDataPath);
				UnityEngine.Debug.Log("Saving player data: " + _playerSaveDataPath);
			}
			else
			{
				PlayerPrefsSerializer.Serialize(data, _playerDataKey);
				UnityEngine.Debug.Log("Saving player data: " + _playerDataKey);
			}
		}
	}
}
