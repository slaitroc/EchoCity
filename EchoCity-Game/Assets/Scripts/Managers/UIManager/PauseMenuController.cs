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
        private Button resumeButton;
        private Button playgroundButton;
        private Button settingsButton;
        private Button feedbackButton;
        private Button quitButton;
        private Button[] buttons;
        private bool _isNavMode = false;
        private bool _showCursor;
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

            resumeButton = _root.Q<Button>("ResumeButton");
            playgroundButton = _root.Q<Button>("PlaygroundButton");
            settingsButton = _root.Q<Button>("SettingsButton");
            feedbackButton = _root.Q<Button>("FeedbackButton");
            quitButton = _root.Q<Button>("QuitButton");
            buttons = new Button[] { resumeButton, playgroundButton, settingsButton, feedbackButton, quitButton };

            yield return null;

            _root.RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (_isNavMode)
                {
                    foreach (var button in buttons)
                        button.pickingMode = PickingMode.Position;
                    DisableFocusHandler();
                    _showCursor = true;
                    _isNavMode = false;
                }
            });

            _root.RegisterCallback<MouseOverEvent>(evt =>
            {
                foreach (var button in buttons)
                    if (button.worldBound.Contains(evt.mousePosition))
                    {
                        button.Focus();
                        break;
                    }
            });

            _root.RegisterCallback<NavigationMoveEvent>(evt =>
            {
                foreach (var button in buttons)
                    button.pickingMode = PickingMode.Ignore;
                _isNavMode = true;
                _showCursor = false;
            });

            if (resumeButton != null) resumeButton.clicked += ResumeClickHandler;
            if (playgroundButton != null) playgroundButton.clicked += PlaygroundClickHandler;
            if (settingsButton != null) settingsButton.clicked += SettingsClickHandler;
            if (feedbackButton != null) feedbackButton.clicked += FeedbackClickHandler;
            if (quitButton != null) quitButton.clicked += QuitClickHandler;
        }

        private void Update()
        {
            MethodsUI.SetCursorState(_showCursor);
        }

        private void DisableFocusHandler()
        {
            foreach (var button in buttons)
                button?.Blur();
        }

        private void ResumeClickHandler() => uiManager.SwitchToPlayState();
        private void SettingsClickHandler() => uiManager.OpenSettingsMenu();
        private void PlaygroundClickHandler() => uiManager.SwitchToInitLevel(SceneEnum.Playground);
        private void FeedbackClickHandler() => uiManager.OpenFeedbackMenu();
        private void QuitClickHandler() => uiManager.SwitchToTitleState();

        private void OnDisable()
        {
            if (resumeButton != null) resumeButton.clicked -= ResumeClickHandler;
            if (playgroundButton != null) playgroundButton.clicked -= PlaygroundClickHandler;
            if (settingsButton != null) settingsButton.clicked -= SettingsClickHandler;
            if (feedbackButton != null) feedbackButton.clicked -= FeedbackClickHandler;
            if (quitButton != null) quitButton.clicked -= QuitClickHandler;

            _showCursor = false;
        }

    }
}
