using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    [RequireComponent(typeof(UIDocument))]
    public class PauseMenuController : MonoBehaviour
    {
        private const string _LOG_TAG = "UI-PauseMenu";
        private const string _LOG_COLOR = "#d900ffff";

        [Header("UI ")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument uiDocument;


        #region Private Fields
        private VisualElement _root;
        private Button _resumeButton;
        private Button _playgroundButton;
        private Button _settingsButton;
        private Button _feedbackButton;
        private Button _quitButton;
        private Button[] _buttons;
        private bool _isNavMode = false;
        private bool _showCursor;
        private bool _showPlaygroundButton = false;
        #endregion

        private void OnEnable()
        {
            if (uiDocument == null) return;
            _root = uiDocument.rootVisualElement;

            StartCoroutine(InitCallbacksNextFrame());

            _showCursor = true;
        }

        IEnumerator InitCallbacksNextFrame()
        {

            _resumeButton = _root.Q<Button>("ResumeButton");
            _playgroundButton = _root.Q<Button>("PlaygroundButton");
            _settingsButton = _root.Q<Button>("SettingsButton");
            _feedbackButton = _root.Q<Button>("FeedbackButton");
            _quitButton = _root.Q<Button>("QuitButton");
            _buttons = new Button[] { _resumeButton, _playgroundButton, _settingsButton, _feedbackButton, _quitButton };

            yield return null;

            _playgroundButton.style.display = DisplayStyle.None;

            _root.RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (_isNavMode)
                {
                    foreach (var button in _buttons)
                        button.pickingMode = PickingMode.Position;
                    DisableFocusHandler();
                    _showCursor = true;
                    _isNavMode = false;
                }
            });

            _root.RegisterCallback<MouseOverEvent>(evt =>
            {
                foreach (var button in _buttons)
                    if (button.worldBound.Contains(evt.mousePosition))
                    {
                        button.Focus();
                        break;
                    }
            });

            _root.RegisterCallback<NavigationMoveEvent>(evt =>
            {
                foreach (var button in _buttons)
                    button.pickingMode = PickingMode.Ignore;
                _isNavMode = true;
                _showCursor = false;
            });

            if (_resumeButton != null) _resumeButton.clicked += ResumeClickHandler;
            if (_playgroundButton != null) _playgroundButton.clicked += PlaygroundClickHandler;
            if (_settingsButton != null) _settingsButton.clicked += SettingsClickHandler;
            if (_feedbackButton != null) _feedbackButton.clicked += FeedbackClickHandler;
            if (_quitButton != null) _quitButton.clicked += QuitClickHandler;
        }

        private void Update()
        {
            MethodsUI.SetCursorState(_showCursor);
        }

        private void DisableFocusHandler()
        {
            foreach (var button in _buttons)
                button?.Blur();
        }

        private void ResumeClickHandler() => uiManager.SwitchToPlayState();
        private void SettingsClickHandler() => uiManager.OpenSettingsMenu();
        private void PlaygroundClickHandler() => uiManager.SwitchToInitLevel(SceneEnum.Playground);
        private void FeedbackClickHandler() => uiManager.OpenFeedbackMenu();
        private void QuitClickHandler() => uiManager.SwitchToTitleState();

        public void ShowPlaygroundButton()
        {
            _playgroundButton.style.display = _showPlaygroundButton ? DisplayStyle.None : DisplayStyle.Flex;
            _showPlaygroundButton = !_showPlaygroundButton;
        }

        private void OnDisable()
        {
            if (_resumeButton != null) _resumeButton.clicked -= ResumeClickHandler;
            if (_playgroundButton != null) _playgroundButton.clicked -= PlaygroundClickHandler;
            if (_settingsButton != null) _settingsButton.clicked -= SettingsClickHandler;
            if (_feedbackButton != null) _feedbackButton.clicked -= FeedbackClickHandler;
            if (_quitButton != null) _quitButton.clicked -= QuitClickHandler;

            _showCursor = false;
        }

    }
}
