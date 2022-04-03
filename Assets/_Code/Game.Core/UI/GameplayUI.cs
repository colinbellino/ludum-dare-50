using System;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Core
{
	public class GameplayUI : MonoBehaviour
	{
		[SerializeField] private GameObject _root;
		[SerializeField] private TMPro.TMP_Text _healthText;

		public bool IsOpened => _root.activeSelf;

		public async UniTask Init()
		{
			await Hide();
		}

		public UniTask Show(float duration = 0.5f)
		{
			_root.SetActive(true);

			// EventSystem.current.SetSelectedGameObject(null);
			// await UniTask.NextFrame();
			// EventSystem.current.SetSelectedGameObject(_levelSelectButton.gameObject);

			return default;
		}

		public UniTask Hide(float duration = 0.5f)
		{
			_root.SetActive(false);

			return default;
		}

		public void SetHealth(int current, int max)
		{
			_healthText.text = $"Health: {current}/{max}";
		}
	}
}
