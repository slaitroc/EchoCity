using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputManager : MonoBehaviour
{

    private const string _LOG_TAG = "UI INPUT MANAGER";
    private const string _LOG_COLOR = "#39e8d1ff";

    [SerializeField] private InputActionAsset inputActionAsset;

    [Header("Invoking Events")]
    [SerializeField] private SOEventVoid pauseEvent;
    [SerializeField] private SOEventVoid enablePlayerActionMapEvent;


    [Header("Observing Events")]
    [SerializeField] private SOEventVoid enableUIActionMapEvent;
    [SerializeField] private SOEventVoid disableUIActionMapEvent;

    private InputActionMap _uiActionMap;

    void Awake()
    {
        InitializeUIBinding();
        _uiActionMap.Disable();


        //Error Logs
        if (inputActionAsset == null)
            Log.E("InputActionAsset is not assigned in UIInputManager", _LOG_COLOR, _LOG_TAG);
    }
    void OnEnable() => SubscribeToEvents();

    void OnDisable() => UnsubscribeFromEvents();

    private void InitializeUIBinding()
    {
        if (inputActionAsset != null)
        {
            _uiActionMap = inputActionAsset.FindActionMap("UI");
            _uiActionMap["ExitPause"].performed += OnExitPause;
        }
    }


    private void SubscribeToEvents()
    {
        if (enableUIActionMapEvent)
        {
            enableUIActionMapEvent.OnEventRaised -= _uiActionMap.Enable;
            enableUIActionMapEvent.OnEventRaised += _uiActionMap.Enable;
        }
        if (disableUIActionMapEvent)
        {
            disableUIActionMapEvent.OnEventRaised -= _uiActionMap.Disable;
            disableUIActionMapEvent.OnEventRaised += _uiActionMap.Disable;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (enableUIActionMapEvent)
            enableUIActionMapEvent.OnEventRaised -= _uiActionMap.Enable;
        if (disableUIActionMapEvent)
            disableUIActionMapEvent.OnEventRaised -= _uiActionMap.Disable;
    }


    private void OnExitPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        pauseEvent?.RaiseEvent();
    }



}