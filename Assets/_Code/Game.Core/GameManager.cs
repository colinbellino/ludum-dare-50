using Cysharp.Threading.Tasks;
using Game.Core.StateMachines.Game;
using Game.Inputs;
using UnityEngine;

namespace Game.Core
{
	public class GameManager : MonoBehaviour
	{
		public static GameSingleton Game { get; private set; }

		[SerializeField] private GameConfig _config;
		[SerializeField] private CameraRig _cameraRig;
		[SerializeField] private GameUI _gameUI;
		[SerializeField] private PauseUI _pauseUI;
		[SerializeField] private OptionsUI _optionsUI;

		private async UniTask Start()
		{
			Game = new GameSingleton();
			Game.Config = _config;
			Game.Controls = new GameControls();
			Game.CameraRig = _cameraRig;
			Game.UI = _gameUI;
			Game.PauseUI = _pauseUI;
			Game.OptionsUI = _optionsUI;
			Game.State = new GameState();
			Game.GameFSM = new GameFSM(_config.DebugStateMachine, Game);
			Game.Save = new Save();

			await Game.GameFSM.Start();
		}

		private void Update()
		{
			Time.timeScale = Game.State.TimeScaleCurrent;
			Game?.GameFSM.Tick();
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (Utils.IsWebGL())
				Silence(!hasFocus);
		}

		private void OnApplicationPause(bool isPaused)
		{
			if (Utils.IsWebGL())
				Silence(isPaused);
		}

		private void Silence(bool silence)
		{
			if (silence)
				AudioHelpers.SetVolume(Game.Config.GameBus, 0);
			else
				AudioHelpers.SetVolume(Game.Config.GameBus, Game.State.PlayerSettings.GameVolume);
		}
	}
}
