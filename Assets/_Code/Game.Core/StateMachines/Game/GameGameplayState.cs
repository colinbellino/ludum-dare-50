using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using FMOD.Studio;
using Unity.Mathematics;
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

			GameManager.Game.UI.ShowGameplay();
			_ = GameManager.Game.UI.FadeOut(2);

			var player = GameObject.Instantiate(GameManager.Game.Config.Player);
			player.transform.position = LevelHelpers.GetRoomCenter(GameManager.Game.State.Level.CurrentRoom);
			GameManager.Game.State.Player = player;

			GameManager.Game.Controls.Gameplay.Enable();
			GameManager.Game.Controls.Gameplay.Move.performed += OnMovePerformed;

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
						Save.SavePlayerSettings(GameManager.Game.State.PlayerSettings);
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

				var player = GameManager.Game.State.Player;
				var level = GameManager.Game.State.Level;
				var roomCenter = LevelHelpers.GetRoomCenter(level.CurrentRoom);
				var roomBounds = new Bounds(roomCenter, GameConfig.ROOM_SIZE);
				if (roomBounds.Contains(player.transform.position) == false)
				{
					var direction = Utils.SnapTo(player.transform.position - roomCenter, 90f);
					direction.y = -direction.y; // Reverse the Y axis because unity's is bottom > top and ours is top > bottom

					var nextRoom = LevelHelpers.GetRoomInDirection(direction, GameManager.Game.State.Level);
					if (nextRoom != null)
					{
						LevelHelpers.TransitionToRoom(GameManager.Game.State.Level, nextRoom);
						var destination = LevelHelpers.GetRoomOrigin(GameManager.Game.State.Level.CurrentRoom);
						GameManager.Game.CameraRig.transform.DOMove(destination, 0.3f);
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
		}

		private void OnMovePerformed(InputAction.CallbackContext context)
		{
			if (GameManager.Game.State.Running == false || GameManager.Game.State.Paused)
				return;
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
			Save.SavePlayerSaveData(GameManager.Game.State.PlayerSaveData);

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
