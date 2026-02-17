using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace EchoCity
{
    public class CreditsController : MonoBehaviour
    {
        [Header("UI ")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument creditsDocument;



        #region Private Fields
        private VisualElement _root;
        private Button _backButton;
        #endregion

        private void OnEnable()
        {
            if (creditsDocument == null) return;
            _root = creditsDocument.rootVisualElement;

            StartCoroutine(InitCallbacksNextFrame());
        }
        IEnumerator InitCallbacksNextFrame()
        {
            _backButton = _root.Q<Button>("BackButton");

            yield return null;

            if (_backButton != null) _backButton.clicked += BackClickHandler;
        }

        private void BackClickHandler() => uiManager.CloseCreditsMenu();

        private void OnDisable()
        {
            if (_backButton != null) _backButton.clicked -= BackClickHandler;
        }
    }
}
