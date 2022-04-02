using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Core.StateMachines.Game;
using Game.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

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
		[SerializeField] private ControlsUI _controlsUI;

		private async UniTask Start()
		{
			Game = new GameSingleton();
			Game.Config = _config;
			Game.Controls = new GameControls();
			Game.CameraRig = _cameraRig;
			Game.UI = _gameUI;
			Game.PauseUI = _pauseUI;
			Game.OptionsUI = _optionsUI;
			Game.ControlsUI = _controlsUI;
			Game.State = new GameState();
			Game.GameFSM = new GameFSM(_config.DebugStateMachine, Game);
			Game.Save = new Save();

			await Game.GameFSM.Start();
		}

		private void Update()
		{
			// Detect last input type (keyboard, gamepad)
			{
				var newInputType = -1;
				if (Gamepad.current != null && Gamepad.current.allControls.Any(x => x.IsActuated()))
				{
					if (Gamepad.current is XInputController)
						newInputType = 1;
					else if (Gamepad.current is DualShockGamepad)
						newInputType = 2;
				}
				else if (Keyboard.current != null && Keyboard.current.allControls.Any(x => x.IsPressed()))
				{
					newInputType = 0;
				}

				if (newInputType > -1 && newInputType != Game.State.CurrentInputType)
				{
					Game.State.CurrentInputType = newInputType;
					Game.ControlsUI.SetInputType(Game.State.CurrentInputType);
				}
			}

			Time.timeScale = Game.State.TimeScaleCurrent;
			Game?.GameFSM.Tick();

			// Stick the UI to the camera
			Game.UI.transform.position = Game.CameraRig.transform.position + Game.CameraRig.Camera.transform.localPosition;
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
