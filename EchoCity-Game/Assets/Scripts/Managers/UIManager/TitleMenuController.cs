using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    [RequireComponent(typeof(UIDocument))]
    public class TitleMenuController : MonoBehaviour
    {
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
        private Button _startGameButton;
        private Button _playgroundButton;
        private Button _settingsButton;
        private Button _quitButton;
        private Button _feedbackButton;
        private Button _creditsButton;
        private Button[] _buttons;
        private bool _isNavMode = false;
        private bool _showCursor;
        private bool _showPlaygroundButton = false;
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
            _startGameButton = _root.Q<Button>("StartGameButton");
            _playgroundButton = _root.Q<Button>("PlaygroundButton");
            _settingsButton = _root.Q<Button>("SettingsButton");
            // quitButton = _root.Q<Button>("QuitButton");
            _feedbackButton = _root.Q<Button>("FeedbackButton");
            _creditsButton = _root.Q<Button>("CreditsButton");
            _buttons = new Button[] { _startGameButton, _playgroundButton, _settingsButton, _feedbackButton, _creditsButton };
            _redBlinkOverlay = _root.Q<VisualElement>("RedBlinkOverlay");
            _blueBlinkOverlay = _root.Q<VisualElement>("BlueBlinkOverlay");

            if (_playgroundButton != null) _playgroundButton.style.display = DisplayStyle.None;

            yield return null;

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

            if (_startGameButton != null) _startGameButton.clicked += StartGameClickHandler;
            if (_playgroundButton != null) _playgroundButton.clicked += PlaygroundClickHandler;
            if (_settingsButton != null) _settingsButton.clicked += SettingsClickHandler;
            if (_quitButton != null) _quitButton.clicked += QuitClickHandler;
            if (_feedbackButton != null) _feedbackButton.clicked += FeedbackClickHandler;
            if (_creditsButton != null) _creditsButton.clicked += CreditsClickHandler;
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
            foreach (var button in _buttons)
                button?.Blur();
        }

        private void StartGameClickHandler()
        {
            _titleMenuContainer.AddToClassList("hide");
            uiManager.SwitchToInitLevel(SceneEnum.InitialNarration);
        }

        private void PlaygroundClickHandler()
        {
            _titleMenuContainer.AddToClassList("hide");
            uiManager.SwitchToInitLevel(SceneEnum.FirstLevel);
        }

        private void SettingsClickHandler()
        {
            uiManager.OpenSettingsMenu();
        }

        private void QuitClickHandler() => Log.DLazy(() => "Quit button clicked", this);

        private void FeedbackClickHandler() => uiManager.OpenFeedbackMenu();
        private void CreditsClickHandler() => uiManager.OpenCreditsMenu();

        public void ShowPlaygroundButton()
        {
            _playgroundButton.style.display = _showPlaygroundButton ? DisplayStyle.None : DisplayStyle.Flex;
            _showPlaygroundButton = !_showPlaygroundButton;
        }

        private void OnDisable()
        {
            if (_startGameButton != null) _startGameButton.clicked -= StartGameClickHandler;
            if (_playgroundButton != null) _playgroundButton.clicked -= PlaygroundClickHandler;
            if (_settingsButton != null) _settingsButton.clicked -= SettingsClickHandler;
            if (_quitButton != null) _quitButton.clicked -= QuitClickHandler;
            if (_feedbackButton != null) _feedbackButton.clicked -= FeedbackClickHandler;
            if (_creditsButton != null) _creditsButton.clicked -= CreditsClickHandler;
            _showCursor = false;
        }
    }
}
