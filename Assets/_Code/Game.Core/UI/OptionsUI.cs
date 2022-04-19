using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System;

namespace Game.Core
{
	public class OptionsUI : MonoBehaviour
	{
		[SerializeField] private GameObject _optionsRoot;
		[SerializeField] private Slider _gameVolumeSlider;
		[SerializeField] private Slider _soundVolumeSlider;
		[SerializeField] private Slider _musicVolumeSlider;
		[SerializeField] private Toggle _fullscreenToggle;
		[SerializeField] private Toggle _assistToggle;
		[SerializeField] private TMP_Dropdown _resolutionsDropdown;
		[SerializeField] private TMP_Dropdown _languagesDropdown;
		[SerializeField] private Button _backButton;

		private List<Resolution> _resolutions;

		public bool IsOpened => _optionsRoot.activeSelf;

		public Action BackClicked;

		public async UniTask Init()
		{
			{
				_resolutions = Screen.resolutions.ToList();

				var options = new List<TMP_Dropdown.OptionData>();
				int selected = 0;
				for (int i = 0; i < _resolutions.Count; ++i)
				{
					var resolution = _resolutions[i];
					if (Screen.currentResolution.width == resolution.height && Screen.currentResolution.height == resolution.width && Screen.currentResolution.refreshRate == resolution.refreshRate)
						selected = i;
					options.Add(new TMP_Dropdown.OptionData($"{resolution.width}x{resolution.height} {resolution.refreshRate}Hz"));
				}
				_resolutionsDropdown.options = options;
				_resolutionsDropdown.value = selected;
				_resolutionsDropdown.template.gameObject.SetActive(false);
				_resolutionsDropdown.onValueChanged.AddListener(OnResolutionChanged);
			}
			_gameVolumeSlider.onValueChanged.AddListener(SetGameVolume);
			_soundVolumeSlider.onValueChanged.AddListener(SetSoundVolume);
			_musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
			_fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
			_assistToggle.onValueChanged.AddListener(ToggleAssistMode);
			{
				var options = new List<TMP_Dropdown.OptionData>();
				int selected = 0;
				for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
				{
					var locale = LocalizationSettings.AvailableLocales.Locales[i];
					if (LocalizationSettings.SelectedLocale == locale)
						selected = i;
					options.Add(new TMP_Dropdown.OptionData(locale.LocaleName));
				}
				_languagesDropdown.options = options;
				_languagesDropdown.value = selected;
				_languagesDropdown.template.gameObject.SetActive(false);
				_languagesDropdown.onValueChanged.AddListener(OnLanguageChanged);
			}
			_backButton.onClick.AddListener(Back);
			await Hide(0);
		}

		public async UniTask Show(float duration = 0.5f)
		{
			GameManager.Game.Controls.Global.Cancel.performed += CancelInputPerformed;

			_optionsRoot.SetActive(true);

			_gameVolumeSlider.value = GameManager.Game.State.PlayerSettings.GameVolume;
			_soundVolumeSlider.value = GameManager.Game.State.PlayerSettings.SoundVolume;
			_musicVolumeSlider.value = GameManager.Game.State.PlayerSettings.MusicVolume;
			_fullscreenToggle.isOn = GameManager.Game.State.PlayerSettings.FullScreen;
			_assistToggle.isOn = GameManager.Game.State.PlayerSettings.AssistMode;

			// We have a button in the browser to do this in WebGL.
			if (Utils.IsWebGL())
			{
				_resolutionsDropdown.gameObject.SetActive(false);
				_fullscreenToggle.gameObject.SetActive(false);
			}

			EventSystem.current.SetSelectedGameObject(null);
			await UniTask.NextFrame();
			EventSystem.current.SetSelectedGameObject(_languagesDropdown.gameObject);
		}

		public UniTask Hide(float duration = 0.5f)
		{
			GameManager.Game.Controls.Global.Cancel.performed -= CancelInputPerformed;

			_optionsRoot.SetActive(false);
			return default;
		}

		private async void CancelInputPerformed(InputAction.CallbackContext obj)
		{
			// Quick hack so we trigger the cancel input again if we go back to the title in the same frame.
			await UniTask.NextFrame();

			Back();
		}

		private void SetGameVolume(float value)
		{
			GameManager.Game.State.PlayerSettings.GameVolume = value;
			AudioHelpers.SetVolume(GameManager.Game.Config.GameBus, value);
			Save.SavePlayerSettings(GameManager.Game.State.PlayerSettings);
		}

		private void SetSoundVolume(float value)
		{
			GameManager.Game.State.PlayerSettings.SoundVolume = value;
			AudioHelpers.SetVolume(GameManager.Game.Config.SoundBus, value);
			Save.SavePlayerSettings(GameManager.Game.State.PlayerSettings);
		}

		private void SetMusicVolume(float value)
		{
			GameManager.Game.State.PlayerSettings.MusicVolume = value;
			AudioHelpers.SetVolume(GameManager.Game.Config.MusicBus, value);
			Save.SavePlayerSettings(GameManager.Game.State.PlayerSettings);
		}

		private void ToggleFullscreen(bool value)
		{
			GameManager.Game.State.PlayerSettings.FullScreen = value;
			Screen.fullScreen = value;
			Save.SavePlayerSettings(GameManager.Game.State.PlayerSettings);
		}

		private void ToggleAssistMode(bool value)
		{
			GameManager.Game.State.PlayerSettings.AssistMode = value;
			Save.SavePlayerSettings(GameManager.Game.State.PlayerSettings);
		}

		private void OnResolutionChanged(int index)
		{
			var resolution = _resolutions[index];
			GameManager.Game.State.PlayerSettings.ResolutionWidth = resolution.width;
			GameManager.Game.State.PlayerSettings.ResolutionHeight = resolution.height;
			GameManager.Game.State.PlayerSettings.ResolutionRefreshRate = resolution.refreshRate;
			Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRate);
			Save.SavePlayerSettings(GameManager.Game.State.PlayerSettings);
		}

		private void OnLanguageChanged(int index)
		{
			var location = LocalizationSettings.AvailableLocales.Locales[index];
			GameManager.Game.State.PlayerSettings.LocaleCode = location.Identifier.Code;
			LocalizationSettings.SelectedLocale = location;
			Save.SavePlayerSettings(GameManager.Game.State.PlayerSettings);
		}

		private async void Back()
		{
			await Hide();
			BackClicked?.Invoke();
		}
	}
}
