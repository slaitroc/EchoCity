using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    [RequireComponent(typeof(UIDocument))]
    public class DeathMenuController : MonoBehaviour
    {
        private const string _LOG_TAG = "UI-DeathMenu";
        private const string _LOG_COLOR = "#ff0000ff";

        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument deathMenuDocument;



        #region Private Fields
        private VisualElement _root;
        private VisualElement _deathMenuPanel;
        private VisualElement _deathBgAnimated;
        private Button _restartButton;
        private Button _playgroundButton;
        private Button _feedbackButton;
        private Button _creditsButton;
        private Button _quitButton;
        private Button[] _buttons;
        private bool _isNavMode = false;
        private bool _showCursor;
        private bool _showPlaygroundButton = false;
        #endregion

        private void OnEnable()
        {
            if (deathMenuDocument == null) return;
            _root = deathMenuDocument.rootVisualElement;
            StartCoroutine(InitCallbacksNextFrame());

            _showCursor = true;
        }

        IEnumerator InitCallbacksNextFrame()
        {
            _deathMenuPanel = _root.Q<VisualElement>("DeathMenuPanel");
            _deathBgAnimated = _root.Q<VisualElement>("DeathBgAnimated");
            _deathBgAnimated.style.translate = new Translate(0, new Length(-100, LengthUnit.Percent));

            _restartButton = _root.Q<Button>("RestartButton");
            _playgroundButton = _root.Q<Button>("PlaygroundButton");
            _feedbackButton = _root.Q<Button>("FeedbackButton");
            _creditsButton = _root.Q<Button>("CreditsButton");

            _quitButton = _root.Q<Button>("QuitButton");
            _buttons = new Button[] { _restartButton, _feedbackButton, _creditsButton, _quitButton };

            if (_playgroundButton != null) _playgroundButton.style.display = DisplayStyle.None;

            yield return null;

            _restartButton.style.display = DisplayStyle.None;

            _deathBgAnimated.style.translate = new Translate(0, 0);

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

            if (_restartButton != null) _restartButton.clicked += RestartGameClickHandler;
            if (_playgroundButton != null) _playgroundButton.clicked += PlaygroundClickHandler;
            if (_feedbackButton != null) _feedbackButton.clicked += FeedbackClickHandler;
            if (_creditsButton != null) _creditsButton.clicked += CreditsClickHandler;
            if (_quitButton != null) _quitButton.clicked += QuitClickHandler;

            _deathMenuPanel.AddToClassList("show");
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

        private void RestartGameClickHandler() => uiManager.SwitchToInitLevel(SceneEnum.FirstLevel);
        private void PlaygroundClickHandler() => uiManager.SwitchToInitLevel(SceneEnum.Playground);
        private void FeedbackClickHandler() => uiManager.OpenFeedbackMenu();
        private void CreditsClickHandler() => uiManager.OpenCreditsMenu();
        private void QuitClickHandler() => uiManager.SwitchToTitleState();

        public void ShowPlaygroundButton()
        {
            _playgroundButton.style.display = _showPlaygroundButton ? DisplayStyle.None : DisplayStyle.Flex;
            _showPlaygroundButton = !_showPlaygroundButton;
        }

        private void OnDisable()
        {
            if (_restartButton != null) _restartButton.clicked -= RestartGameClickHandler;
            if (_feedbackButton != null) _feedbackButton.clicked -= FeedbackClickHandler;
            if (_creditsButton != null) _creditsButton.clicked -= CreditsClickHandler;
            if (_quitButton != null) _quitButton.clicked -= QuitClickHandler;
            if (_playgroundButton != null) _playgroundButton.clicked -= PlaygroundClickHandler;

            _deathMenuPanel.RemoveFromClassList("show");
            _showCursor = false;
        }
    }
}
