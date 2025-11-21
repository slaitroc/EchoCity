using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PauseMenu : MonoBehaviour
{
    #region Serialized Fields
    [Header("Invoking events")]
    [SerializeField] private SOEventVoid pauseEvent;
    [SerializeField] private SOEventVoid settingsEvent;
    [SerializeField] private SOEventVoid quitToTitleEvent;
    
    [Header("UI Elements")]
    [SerializeField] private UIDocument pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitToTitleButton;
    #endregion
    
    #region Private Fields
    private VisualElement _root;
    private bool _isKeyboardMode;
    private bool _suppressNextNavigation;
    
    private Button _currentHoveredButton;
    private Button _lastHoveredButton;
    #endregion

    private void OnEnable()
    {
        if (pauseMenu == null) return;
        
        resumeButton = pauseMenu.rootVisualElement.Q<Button>("ResumeButton");
        settingsButton = pauseMenu.rootVisualElement.Q<Button>("SettingsButton");
        quitToTitleButton = pauseMenu.rootVisualElement.Q<Button>("QuitToTitleButton");


        if (resumeButton != null)
        {
            resumeButton.clicked += OnResume;
        }
        
        if (settingsButton != null)
        {
            settingsButton.clicked += OnSettings;
        }
        
        if (quitToTitleButton != null)
        {
            quitToTitleButton.clicked += OnQuitToTitle;
        }
        

        _root = pauseMenu.rootVisualElement;
        _root.RegisterCallback<NavigationMoveEvent>(OnNavigationMove, TrickleDown.TrickleDown);
        
        foreach (var button in _root.Query<Button>().ToList())
        {
            button.RegisterCallback<MouseEnterEvent>(evt => {_currentHoveredButton = button;});
            button.RegisterCallback<MouseLeaveEvent>(evt => {_currentHoveredButton = null;});
        }
        
        _isKeyboardMode = false;
        ShowCursor();
    }

    private void Update()
    {
        HandleKeyBoard();
        HandleMouse();
    }

    private void OnDisable()
    {
        if (resumeButton != null) resumeButton.clicked -= OnResume;
        if (settingsButton != null) settingsButton.clicked -= OnSettings;
        if (quitToTitleButton != null) quitToTitleButton.clicked -= OnQuitToTitle;
        _root.UnregisterCallback<NavigationMoveEvent>(OnNavigationMove);
        
        HideCursor();
    }

    private void OnResume()
    {
        Log.D("Resume button clicked", "violet", "UI MANAGER");
        pauseEvent.RaiseEvent();
    }
    
    private void OnSettings()
    {
        Log.D("Settings button clicked", "violet", "UI MANAGER");
        settingsEvent.RaiseEvent();
    }
    
    private void OnQuitToTitle()
    {
        Log.D("Quit To Title button clicked", "violet", "UI MANAGER");
        quitToTitleEvent.RaiseEvent();
    }

    
    private void SwitchToKeyboardMode()
    {
        if (_isKeyboardMode) return;
        
        _isKeyboardMode = true;
        Log.D("Switched to Keyboard mode", "cyan", "UI MANAGER");
        
        HideCursor();
        
        Log.D("Cursor hidden", "cyan", "UI MANAGER");
        
        
        if (_currentHoveredButton != null)
        {
            _currentHoveredButton.pickingMode = PickingMode.Ignore;
            _lastHoveredButton = _currentHoveredButton;
        }
        
        if (_root.focusController?.focusedElement == null && resumeButton != null)
        {
            resumeButton.Focus();
            _suppressNextNavigation = true;
        }
        
    }
    
    private void SwitchToMouseMode()
    {
        if (!_isKeyboardMode) return;
        
        _isKeyboardMode = false;
        Log.D("Switched to Mouse mode", "cyan", "UI MANAGER");
        
        ShowCursor();

        if (_lastHoveredButton != null)
        {
            _lastHoveredButton.pickingMode = PickingMode.Position;
            _lastHoveredButton = null;
        }
        
        var focusedElement = _root.focusController?.focusedElement as VisualElement;
        focusedElement?.Blur();

    }
    
    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HandleKeyBoard()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) SwitchToKeyboardMode();
    }

    private void HandleMouse()
    {
        if (Mouse.current == null) return;
        
        // Mouse movement
        var delta = Mouse.current.delta.ReadValue();
        bool moved = delta.sqrMagnitude > 0.01f;

        // Mouse click
        bool clicked = Mouse.current.leftButton.wasPressedThisFrame ||
                       Mouse.current.rightButton.wasPressedThisFrame ||
                       Mouse.current.middleButton.wasPressedThisFrame;

        if (moved || clicked)
        {
            SwitchToMouseMode();
        }
    }
    
    private void OnNavigationMove(NavigationMoveEvent evt)
    {
        if (_suppressNextNavigation)
        {
            _suppressNextNavigation = false;
            evt.PreventDefault();
            evt.StopImmediatePropagation(); // Disables the first frame navigation event to keep the Resume button focused
            return;
        }
    }
}
