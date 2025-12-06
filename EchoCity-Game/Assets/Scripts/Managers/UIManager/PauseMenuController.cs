using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuController : MonoBehaviour
{
    private const string _LOG_TAG = "UI-PauseMenu";
    private const string _LOG_COLOR = "#d900ffff";

    [Header("UI ")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private UIDocument uiDocument;

    [Header("Invoking events")]
    [SerializeField] private SOEventVoid pauseGameEvent;
    [SerializeField] private SOEventVoid pauseMenuEvent;
    [SerializeField] private SOEventVoid openSettingsMenuEvent;
    [SerializeField] private SOEventVoid quitToTitleEvent;

    #region Private Fields
    private VisualElement _root;
    private Button resumeButton;
    private Button settingsButton;
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

        uiManager.EnableUIActionMap();
        _showCursor = true;
    }
    
    IEnumerator InitCallbacksNextFrame()
    {

        resumeButton = _root.Q<Button>("ResumeButton");
        settingsButton = _root.Q<Button>("SettingsButton");
        quitButton = _root.Q<Button>("QuitButton");
        buttons = new Button[] { resumeButton, settingsButton, quitButton };

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
        if (settingsButton != null) settingsButton.clicked += SettingsClickHandler;
        if (quitButton != null) quitButton.clicked += QuitClickHandler;
    }

    private void OnDisable()
    {
        if (resumeButton != null) resumeButton.clicked -= ResumeClickHandler;
        if (settingsButton != null) settingsButton.clicked -= SettingsClickHandler;
        if (quitButton != null) quitButton.clicked -= QuitClickHandler;

        uiManager.DisableUIActionMap();
        uiManager.EnablePlayerActionMap();
        _showCursor = false;
    }

    void Update()
    {
        MethodsUI.SetCursorState(_showCursor);
    }

    private void DisableFocusHandler()
    {
        foreach (var button in buttons)
            button?.Blur();
    }

    private void ResumeClickHandler()
    {
        pauseMenuEvent?.RaiseEvent();
        pauseGameEvent?.RaiseEvent();
    }

    private void SettingsClickHandler()
    {
        uiManager.OpenSettingsMenuHandler();
        openSettingsMenuEvent?.RaiseEvent();
    }

    private void QuitClickHandler() => quitToTitleEvent?.RaiseEvent();

}
