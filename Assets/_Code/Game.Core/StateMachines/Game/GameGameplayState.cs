using System;
using System.IO;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMOD.Studio;
using NesScripts.Controls.PathFind;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Tilemaps;

namespace Game.Core.StateMachines.Game
{
	public class GameGameplayState : BaseGameState
	{
		public GameGameplayState(GameFSM fsm, GameSingleton game) : base(fsm, game) { }

		public override async UniTask Enter()
		{
			await base.Enter();

			_state.Running = true;

			_controls.Gameplay.Enable();
			_controls.Gameplay.Move.performed += OnMovePerformed;

			_ui.ShowGameplay();
			_ = _ui.FadeOut(2);

			_state.LevelMusic.setPitch(_state.TimeScaleCurrent);
			_state.LevelMusic.getPlaybackState(out var state);
			if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
				_state.LevelMusic.start();
		}

		public override void Tick()
		{
			base.Tick();

			if (_state.Running)
			{
				if (_controls.Global.Pause.WasPerformedThisFrame())
				{
					if (_state.Paused)
					{
						_state.TimeScaleCurrent = _state.TimeScaleDefault;
						_state.Paused = false;
						_game.PauseUI.Hide();
						_state.PauseSnapshot.stop(STOP_MODE.ALLOWFADEOUT);
					}
					else
					{
						_state.TimeScaleCurrent = 0f;
						_state.Paused = true;
						_ = _game.PauseUI.Show();
						_state.PauseSnapshot.start();
					}
				}

				if (_controls.Global.Cancel.WasPerformedThisFrame())
				{
					if (_game.OptionsUI.IsOpened)
					{
						_game.OptionsUI.Hide();
						_game.PauseUI.SelectOptionsGameObject();
						_game.Save.SavePlayerSettings(_game.State.PlayerSettings);
					}
				}

				if (_controls.Gameplay.Reset.WasPerformedThisFrame())
				{
					_fsm.Fire(GameFSM.Triggers.Retry);
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

		public override async UniTask Exit()
		{
			await base.Exit();

			_controls.Gameplay.Disable();
			_controls.Gameplay.Move.performed -= OnMovePerformed;

			_state.TimeScaleCurrent = _state.TimeScaleDefault;
			_state.Running = false;
			_state.Paused = false;
			_state.PauseSnapshot.stop(STOP_MODE.ALLOWFADEOUT);

			await _ui.FadeIn(Color.black);
			await _ui.HideLevelName(0);
			_ui.HideGameplay();
			await _game.PauseUI.Hide(0);
			await _game.OptionsUI.Hide(0);

			_state.Entities.Clear();
		}

		private void OnMovePerformed(InputAction.CallbackContext context)
		{
			if (_state.Running == false || _state.Paused)
				return;

			//
		}

		private void Victory()
		{
			UnityEngine.Debug.Log("End of the game reached.");
			_state.LevelMusic.stop(STOP_MODE.ALLOWFADEOUT);
			_fsm.Fire(GameFSM.Triggers.Won);
		}

		private void NextLevel()
		{
			_game.State.PlayerSaveData.ClearedLevels.Add(_state.CurrentLevelIndex);
			_game.Save.SavePlayerSaveData(_game.State.PlayerSaveData);

			_state.CurrentLevelIndex += 1;

			if (_state.CurrentLevelIndex >= _config.Levels.Length)
			{
				Victory();
				return;
			}

			_state.Running = false;
			_fsm.Fire(GameFSM.Triggers.NextLevel);
		}
	}
}
