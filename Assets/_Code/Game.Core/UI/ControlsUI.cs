using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Core
{
	public class ControlsUI : MonoBehaviour
	{
		[SerializeField] private GameObject _root;
		[SerializeField] private Button _closeButton;
		[SerializeField] private GameObject _xbox;
		[SerializeField] private GameObject _ps5;

		public bool IsOpened => _root.activeSelf;

		public async UniTask Init()
		{
			await Hide(0);
		}

		public async UniTask Show(float duration = 0.5f)
		{
			GameManager.Game.Controls.Global.Cancel.performed += CancelInputPerformed;
			_closeButton.onClick.AddListener(CloseButtonClick);

			_root.SetActive(true);

			EventSystem.current.SetSelectedGameObject(null);
			await UniTask.NextFrame();
			// EventSystem.current.SetSelectedGameObject(_languagesDropdown.gameObject);
		}

		public UniTask Hide(float duration = 0.5f)
		{
			GameManager.Game.Controls.Global.Cancel.performed -= CancelInputPerformed;
			_closeButton.onClick.RemoveListener(CloseButtonClick);

			_root.SetActive(false);
			return default;
		}

		private async void CancelInputPerformed(InputAction.CallbackContext obj)
		{
			UnityEngine.Debug.Log("CancelInputPerformed");
		}

		private void CloseButtonClick()
		{
			UnityEngine.Debug.Log("CloseButtonClick");
		}
	}
}
