using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    [RequireComponent(typeof(UIDocument))]
    public class TitleMenuController : MonoBehaviour
    {
        private const string _LOG_TAG = "UI-TitleMenu";
        private const string _LOG_COLOR = "#2600ffff";

        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument titleMenuDocument;


#pragma warning disable CS0414
        [Header("Delays")]
        [SerializeField] private float startGameDelay = 1f;
#pragma warning restore CS0414


        #region Private Fields
        private VisualElement _root;
        private VisualElement _titleMenuContainer;
        private VisualElement _redBlinkOverlay;
        private VisualElement _blueBlinkOverlay;
        private Button startGameButton;
        private Button playgroundButton;
        private Button settingsButton;
        private Button quitButton;
        private Button feedbackButton;
        private Button[] buttons;
        private bool _isNavMode = false;
        private bool _showCursor;
        #endregion

        private void OnEnable()
        {
            if (titleMenuDocument == null) return;
            _root = titleMenuDocument.rootVisualElement;

            StartCoroutine(InitCallbacksNextFrame());
            StartCoroutine(RedBlinkLoop());
            StartCoroutine(BlueBlinkLoop());

            _showCursor = true;
        }

        IEnumerator InitCallbacksNextFrame()
        {
            _titleMenuContainer = _root.Q<VisualElement>("TitleMenuContainer");
            startGameButton = _root.Q<Button>("StartGameButton");
            playgroundButton = _root.Q<Button>("PlaygroundButton");
            settingsButton = _root.Q<Button>("SettingsButton");
            // quitButton = _root.Q<Button>("QuitButton");
            feedbackButton = _root.Q<Button>("FeedbackButton");
            buttons = new Button[] { startGameButton, playgroundButton, settingsButton, feedbackButton };
            _redBlinkOverlay = _root.Q<VisualElement>("RedBlinkOverlay");
            _blueBlinkOverlay = _root.Q<VisualElement>("BlueBlinkOverlay");

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

            if (startGameButton != null) startGameButton.clicked += StartGameClickHandler;
            if (playgroundButton != null) playgroundButton.clicked += PlaygroundClickHandler;
            if (settingsButton != null) settingsButton.clicked += SettingsClickHandler;
            if (quitButton != null) quitButton.clicked += QuitClickHandler;
            if (feedbackButton != null) feedbackButton.clicked += FeedbackClickHandler;
        }

        IEnumerator RedBlinkLoop()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);
                float blinkDuration = Random.Range(1.5f, 2f);
                yield return StartCoroutine(RedBlinkEffectCoroutine(blinkDuration));
            }
        }
        IEnumerator BlueBlinkLoop()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1.5f);
                float blinkDuration = Random.Range(2f, 2.5f);
                yield return StartCoroutine(BlueBlinkEffectCoroutine(blinkDuration));
            }
        }

        IEnumerator RedBlinkEffectCoroutine(float duration)
        {
            _redBlinkOverlay.AddToClassList("active");
            yield return new WaitForSecondsRealtime(duration);
            _redBlinkOverlay.RemoveFromClassList("active");
        }

        IEnumerator BlueBlinkEffectCoroutine(float duration)
        {
            _blueBlinkOverlay.AddToClassList("active");
            yield return new WaitForSecondsRealtime(duration);
            _blueBlinkOverlay.RemoveFromClassList("active");
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

        private void StartGameClickHandler()
        {
            _titleMenuContainer.AddToClassList("hide");
            uiManager.SwitchToInitLevel(SceneEnum.Level1);
        }

        private void PlaygroundClickHandler()
        {
            _titleMenuContainer.AddToClassList("hide");
            uiManager.SwitchToInitLevel(SceneEnum.Playground);
        }

        private void SettingsClickHandler()
        {
            uiManager.OpenSettingsMenu();
        }

        private void QuitClickHandler() => Log.DLazy(() => "Quit button clicked", _LOG_TAG, _LOG_COLOR);

        private void FeedbackClickHandler() => uiManager.OpenFeedbackMenu();

        private void OnDisable()
        {
            if (startGameButton != null) startGameButton.clicked -= StartGameClickHandler;
            if (playgroundButton != null) playgroundButton.clicked -= PlaygroundClickHandler;
            if (settingsButton != null) settingsButton.clicked -= SettingsClickHandler;
            if (quitButton != null) quitButton.clicked -= QuitClickHandler;
            if (feedbackButton != null) feedbackButton.clicked -= FeedbackClickHandler;

            _showCursor = false;
        }
    }
}
