using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class DeathScreenController : MonoBehaviour
{
    private const string _LOG_TAG = "UI-DeathScreen";
    private const string _LOG_COLOR = "#ff0000ff";

    [Header("UI")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private UIDocument deathScreenDocument;

    [Header("Invoking events")]
    [SerializeField] private SOEventVoid deathEvent;
    [SerializeField] private SOEventVoid restartGameEvent;
    [SerializeField] private SOEventVoid quitToTitleEvent;

    [Header("Delays")]
    [SerializeField] private float restartGameDelay = 1f;


    #region Private Fields
    private VisualElement _root;
    private VisualElement _deathScreenPanel;
    private VisualElement _deathBgAnimated;
    private Button _restartButton;
    private Button _quitButton;
    private Button[] buttons;
    private bool _isNavMode = false;
    private bool _showCursor;
    #endregion

    private void OnEnable()
    {
        if (deathScreenDocument == null) return;
        _root = deathScreenDocument.rootVisualElement;
        StartCoroutine(InitCallbacksNextFrame());

        _showCursor = true;
        uiManager.EnableUIActionMap();
    }

    IEnumerator InitCallbacksNextFrame()
    {
        _deathScreenPanel = _root.Q<VisualElement>("DeathScreenPanel");
        _deathBgAnimated = _root.Q<VisualElement>("DeathBgAnimated");
        _deathBgAnimated.style.translate = new Translate(0, new Length(-100, LengthUnit.Percent));

        _restartButton = _root.Q<Button>("RestartButton");
        _quitButton = _root.Q<Button>("QuitButton");
        buttons = new Button[] { _restartButton, _quitButton };

        yield return null;

        _deathBgAnimated.style.translate = new Translate(0, 0);

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
        if (_quitButton != null) _quitButton.clicked += QuitClickHandler;

        _deathScreenPanel.AddToClassList("show");
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

    private void RestartGameClickHandler() => StartCoroutine(StartGameDelay(restartGameDelay));

    private void QuitClickHandler() => quitToTitleEvent?.RaiseEvent();

    IEnumerator StartGameDelay(float delay)
    {
        uiManager.EnablePlayerActionMap();
        _showCursor = false;

        deathEvent?.RaiseEvent();
        restartGameEvent?.RaiseEvent();

        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (_restartButton != null) _restartButton.clicked -= RestartGameClickHandler;
        if (_quitButton != null) _quitButton.clicked -= QuitClickHandler;

        _deathScreenPanel.RemoveFromClassList("show");

        uiManager.DisableUIActionMap();
        uiManager.EnablePlayerActionMap();
    }
}
