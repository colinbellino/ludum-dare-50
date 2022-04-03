using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Game.Core
{
	public class GameplayUI : MonoBehaviour
	{
		[SerializeField] private GameObject _root;
		[SerializeField] private TMPro.TMP_Text _healthText;
		[SerializeField] private RectTransform _healthCurrentFill;
		[SerializeField] private RectTransform _healthTempFill;
		[SerializeField] private RectTransform _dashIcon;

		private float _currentHealthDefaultWidth;
		private float _tempHealthDefaultWidth;
		private float _previousCurrentHealth;
		private float _tempHealth;
		private TweenerCore<float, float, FloatOptions> _tempHealthTween;

		public bool IsOpened => _root.activeSelf;

		public async UniTask Init()
		{
			_currentHealthDefaultWidth = _healthCurrentFill.sizeDelta.x;
			_tempHealthDefaultWidth = _healthTempFill.sizeDelta.x;
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
			// _healthText.text = $"Health: {current}/{max}";

			var currentPercentage = (float)current / max;

			if (current < _previousCurrentHealth)
			{
				var loss = _previousCurrentHealth - current;
				var tempPercentage = (current + loss) / max;
				_healthTempFill.sizeDelta = new Vector2(tempPercentage * _tempHealthDefaultWidth, _healthTempFill.sizeDelta.y);

				_tempHealth = current + loss;

				_tempHealthTween = DOTween.To(() => _tempHealth, x => _tempHealth = x, current, 1f)
					.OnUpdate(() =>
					{
						_healthTempFill.sizeDelta = new Vector2(_tempHealth / max * _tempHealthDefaultWidth, _healthTempFill.sizeDelta.y);
					});
			}

			_healthCurrentFill.sizeDelta = new Vector2(currentPercentage * _currentHealthDefaultWidth, _healthCurrentFill.sizeDelta.y);

			_previousCurrentHealth = current;
		}

		public void SetDash(bool value)
		{
			_dashIcon.gameObject.SetActive(value);
		}
	}
}
