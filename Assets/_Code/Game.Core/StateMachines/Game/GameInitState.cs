using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using static Game.Core.Utils;

namespace Game.Core.StateMachines.Game
{
	public class GameInitState : IState
	{
		public GameFSM FSM;

		public async UniTask Enter()
		{
			_ = GameManager.Game.UI.FadeIn(Color.black, 0);

			FMODUnity.RuntimeManager.LoadBank("SFX", loadSamples: true);

			GameManager.Game.State.GameBus = FMODUnity.RuntimeManager.GetBus("bus:/Game");
			GameManager.Game.State.MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/Game/Music");
			GameManager.Game.State.SoundBus = FMODUnity.RuntimeManager.GetBus("bus:/Game/SFX");
			GameManager.Game.State.TitleMusic = FMODUnity.RuntimeManager.CreateInstance(GameManager.Game.Config.MusicTitle);
			GameManager.Game.State.LevelMusic = FMODUnity.RuntimeManager.CreateInstance(GameManager.Game.Config.MusicMain);
			GameManager.Game.State.PauseSnapshot = FMODUnity.RuntimeManager.CreateInstance(GameManager.Game.Config.SnapshotPause);
			GameManager.Game.State.TimeScaleCurrent = GameManager.Game.State.TimeScaleDefault = 1f;
			GameManager.Game.State.Random = new Unity.Mathematics.Random();
			GameManager.Game.State.Random.InitState((uint)Random.Range(0, int.MaxValue));
			GameManager.Game.State.DebugLevels = new Level[0];
			GameManager.Game.State.AllLevels = GameManager.Game.Config.Levels;

			while (LocalizationSettings.InitializationOperation.IsDone == false)
				await UniTask.NextFrame();

			GameManager.Game.State.PlayerSettings = GameManager.Game.Save.LoadPlayerSettings();
			GameManager.Game.State.PlayerSaveData = GameManager.Game.Save.LoadPlayerSaveData();
			SetPlayerSettings(GameManager.Game.State.PlayerSettings);

			GameManager.Game.Controls.Global.Enable();

			await GameManager.Game.UI.Init(GameManager.Game);
			await GameManager.Game.PauseUI.Init(GameManager.Game);
			await GameManager.Game.OptionsUI.Init(GameManager.Game);

			if (IsDevBuild())
			{
				if (GameManager.Game.Config.DebugLevels)
				{
					GameManager.Game.State.DebugLevels = Resources.LoadAll<Level>("Levels/Debug");
					GameManager.Game.State.AllLevels = new Level[GameManager.Game.Config.Levels.Length + GameManager.Game.State.DebugLevels.Length];
					GameManager.Game.Config.Levels.CopyTo(GameManager.Game.State.AllLevels, 0);
					GameManager.Game.State.DebugLevels.CopyTo(GameManager.Game.State.AllLevels, GameManager.Game.Config.Levels.Length);
				}

				GameManager.Game.UI.ShowDebug();

				if (GameManager.Game.Config.LockFPS > 0)
				{
					Debug.Log($"Locking FPS to {GameManager.Game.Config.LockFPS}");
					Application.targetFrameRate = GameManager.Game.Config.LockFPS;
					QualitySettings.vSyncCount = 1;
				}
				else
				{
					Application.targetFrameRate = 999;
					QualitySettings.vSyncCount = 0;
				}
			}

			FSM.Fire(GameFSM.Triggers.Done);
		}

		public void Tick() { }

		public void FixedTick() { }

		public UniTask Exit() { return default; }

		private void SetPlayerSettings(PlayerSettings playerSettings)
		{
			GameManager.Game.State.GameBus.setVolume(playerSettings.GameVolume);
			GameManager.Game.State.MusicBus.setVolume(playerSettings.MusicVolume);
			GameManager.Game.State.SoundBus.setVolume(playerSettings.SoundVolume);
			Screen.SetResolution(playerSettings.ResolutionWidth, playerSettings.ResolutionHeight, playerSettings.FullScreen, playerSettings.ResolutionRefreshRate);
			LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(playerSettings.LocaleCode);
		}
	}
}
