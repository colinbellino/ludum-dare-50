using Cysharp.Threading.Tasks;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core.StateMachines.Game
{
	public class GameGameplayState : IState
	{
		public GameFSM FSM;

		public UniTask Enter()
		{
			GameManager.Game.State.Running = true;

			GameManager.Game.Controls.Gameplay.Enable();
			GameManager.Game.Controls.Gameplay.Move.performed += OnMovePerformed;

			GameManager.Game.UI.ShowGameplay();
			_ = GameManager.Game.UI.FadeOut(2);

			GameManager.Game.State.LevelMusic.setPitch(GameManager.Game.State.TimeScaleCurrent);
			GameManager.Game.State.LevelMusic.getPlaybackState(out var state);
			if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
				GameManager.Game.State.LevelMusic.start();

			return default;
		}

		public void Tick()
		{
			if (GameManager.Game.State.Running)
			{
				if (GameManager.Game.Controls.Global.Pause.WasPerformedThisFrame())
				{
					if (GameManager.Game.State.Paused)
					{
						GameManager.Game.State.TimeScaleCurrent = GameManager.Game.State.TimeScaleDefault;
						GameManager.Game.State.Paused = false;
						GameManager.Game.PauseUI.Hide();
						GameManager.Game.State.PauseSnapshot.stop(STOP_MODE.ALLOWFADEOUT);
					}
					else
					{
						GameManager.Game.State.TimeScaleCurrent = 0f;
						GameManager.Game.State.Paused = true;
						_ = GameManager.Game.PauseUI.Show();
						GameManager.Game.State.PauseSnapshot.start();
					}
				}

				if (GameManager.Game.Controls.Global.Cancel.WasPerformedThisFrame())
				{
					if (GameManager.Game.OptionsUI.IsOpened)
					{
						GameManager.Game.OptionsUI.Hide();
						GameManager.Game.PauseUI.SelectOptionsGameObject();
						GameManager.Game.Save.SavePlayerSettings(GameManager.Game.State.PlayerSettings);
					}
				}

				if (GameManager.Game.Controls.Gameplay.Reset.WasPerformedThisFrame())
				{
					FSM.Fire(GameFSM.Triggers.Retry);
				}

				if (Utils.IsDevBuild())
				{
					if (Keyboard.current.f1Key.wasReleasedThisFrame)
					{
						NextLevel();
					}

					if (Keyboard.current.f2Key.wasReleasedThisFrame)
					{
						// Victory();
					}
				}
			}
		}

		public void FixedTick() { }

		public async UniTask Exit()
		{
			GameManager.Game.Controls.Gameplay.Disable();
			GameManager.Game.Controls.Gameplay.Move.performed -= OnMovePerformed;

			GameManager.Game.State.TimeScaleCurrent = GameManager.Game.State.TimeScaleDefault;
			GameManager.Game.State.Running = false;
			GameManager.Game.State.Paused = false;
			GameManager.Game.State.PauseSnapshot.stop(STOP_MODE.ALLOWFADEOUT);

			await GameManager.Game.UI.FadeIn(Color.black);
			await GameManager.Game.UI.HideLevelName(0);
			GameManager.Game.UI.HideGameplay();
			await GameManager.Game.PauseUI.Hide(0);
			await GameManager.Game.OptionsUI.Hide(0);

			GameManager.Game.State.Entities.Clear();
		}

		private void OnMovePerformed(InputAction.CallbackContext context)
		{
			if (GameManager.Game.State.Running == false || GameManager.Game.State.Paused)
				return;

			//
		}

		private void Victory()
		{
			UnityEngine.Debug.Log("End of the game reached.");
			GameManager.Game.State.LevelMusic.stop(STOP_MODE.ALLOWFADEOUT);
			FSM.Fire(GameFSM.Triggers.Won);
		}

		private void NextLevel()
		{
			GameManager.Game.State.PlayerSaveData.ClearedLevels.Add(GameManager.Game.State.CurrentLevelIndex);
			GameManager.Game.Save.SavePlayerSaveData(GameManager.Game.State.PlayerSaveData);

			GameManager.Game.State.CurrentLevelIndex += 1;

			if (GameManager.Game.State.CurrentLevelIndex >= GameManager.Game.Config.Levels.Length)
			{
				Victory();
				return;
			}

			GameManager.Game.State.Running = false;
			FSM.Fire(GameFSM.Triggers.NextLevel);
		}
	}
}
