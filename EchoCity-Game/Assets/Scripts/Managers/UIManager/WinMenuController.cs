using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoCity
{
    [RequireComponent(typeof(UIDocument))]
    public class WinMenuController : MonoBehaviour
    {
        private const string _LOG_TAG = "UI-WinMenu";
        private const string _LOG_COLOR = "#16d442ff";

        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIDocument winMenuDocument;



        #region Private Fields
        private VisualElement _root;
        private VisualElement _winMenuPanel;
        private VisualElement _winBgAnimated;
        private Button _restartButton;
        private Button _feedbackButton;
        private Button _quitButton;
        private Button[] buttons;
        private bool _isNavMode = false;
        private bool _showCursor;
        #endregion

        private void OnEnable()
        {
            if (winMenuDocument == null) return;
            _root = winMenuDocument.rootVisualElement;
            StartCoroutine(InitCallbacksNextFrame());

            _showCursor = true;
        }

        IEnumerator InitCallbacksNextFrame()
        {
            _winMenuPanel = _root.Q<VisualElement>("WinMenuPanel");
            // _winBgAnimated = _root.Q<VisualElement>("WinBgAnimated");
            // _winBgAnimated.style.translate = new Translate(0, new Length(-100, LengthUnit.Percent));

            _restartButton = _root.Q<Button>("RestartButton");
            _feedbackButton = _root.Q<Button>("FeedbackButton");
            _quitButton = _root.Q<Button>("QuitButton");
            buttons = new Button[] { _restartButton, _feedbackButton, _quitButton };

            yield return null;

            _restartButton.style.display = DisplayStyle.None;

            // _winBgAnimated.style.translate = new Translate(0, 0);

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

            if (_restartButton != null) _restartButton.clicked += RestartGameClickHandler;
            if (_feedbackButton != null) _feedbackButton.clicked += FeedbackClickHandler;
            if (_quitButton != null) _quitButton.clicked += QuitClickHandler;

            _winMenuPanel.AddToClassList("show");
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

        private void RestartGameClickHandler()
        {
            _showCursor = false;
            uiManager.SwitchToInitLevel(SceneEnum.FirstLevel);
        }

        private void FeedbackClickHandler() => uiManager.OpenFeedbackMenu();
        private void QuitClickHandler() => uiManager.SwitchToTitleState();

        private void OnDisable()
        {
            if (_restartButton != null) _restartButton.clicked -= RestartGameClickHandler;
            if (_feedbackButton != null) _feedbackButton.clicked -= FeedbackClickHandler;
            if (_quitButton != null) _quitButton.clicked -= QuitClickHandler;

            _winMenuPanel.RemoveFromClassList("show");
        }
    }

}