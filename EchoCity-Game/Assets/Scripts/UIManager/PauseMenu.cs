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
    private Vector2 _lastMousePosition;
    private bool _isKeyboardMode;
    
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
        _root.RegisterCallback<KeyDownEvent>(OnKeyDown);
        _root.RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
        _root.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        _root.RegisterCallback<MouseDownEvent>(OnMouseClick);
        
        // _lastMousePosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        _isKeyboardMode = false;

        ShowCursor();

        
        foreach (var button in _root.Query<Button>().ToList())
        {
            button.RegisterCallback<MouseEnterEvent>(evt => {_currentHoveredButton = button;});
            button.RegisterCallback<MouseLeaveEvent>(evt => {_currentHoveredButton = null;});
        }
        
    }

    private void OnDisable()
    {
        if (resumeButton != null) resumeButton.clicked -= OnResume;
        if (settingsButton != null) settingsButton.clicked -= OnSettings;
        if (quitToTitleButton != null) quitToTitleButton.clicked -= OnQuitToTitle;
        
        if (_root != null)
        {
            // _root.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            _root.UnregisterCallback<NavigationMoveEvent>(OnNavigationMove);
            _root.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            _root.UnregisterCallback<MouseDownEvent>(OnMouseClick);
        }
        
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

    
    // TODO: Fix cursor not hiding when using keyboard and not showing back when moving mouse
    private void SwitchToKeyboardMode()
    {
        if (_isKeyboardMode) return;
        
        _isKeyboardMode = true;
        Log.D("Switched to Keyboard mode", "cyan", "UI MANAGER");
        
        // HideCursor();
        
        Log.D("Cursor hidden", "cyan", "UI MANAGER");
        
        
        if (_currentHoveredButton != null)
        {
            _currentHoveredButton.pickingMode = PickingMode.Ignore;
            _lastHoveredButton = _currentHoveredButton;
        }
        
    }
    
    private void SwitchToMouseMode()
    {
        if (!_isKeyboardMode) return;
        
        _isKeyboardMode = false;
        Log.D("Switched to Mouse mode", "cyan", "UI MANAGER");
        
        // ShowCursor();
        
        if (_lastHoveredButton != null) _lastHoveredButton.pickingMode = PickingMode.Position;
        
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
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        SwitchToKeyboardMode();
    }
    
    private void OnNavigationMove(NavigationMoveEvent evt)
    {
        SwitchToKeyboardMode();
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        //
        // Vector2 currentMousePosition = evt.mousePosition;
        //
        // if (Vector2.Distance(_lastMousePosition, currentMousePosition) > 1f)
        // {
        //     _lastMousePosition = currentMousePosition;
        //     SwitchToMouseMode();
        // }
        
        SwitchToMouseMode();
    }
    
    private void OnMouseClick(MouseDownEvent evt)
    {
        SwitchToMouseMode();
    }

}
