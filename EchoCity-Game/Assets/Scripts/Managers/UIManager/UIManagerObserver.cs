using UnityEngine;

public class UIManagerObserver : MonoBehaviour
{
    [Header("Observed Events")]
    [SerializeField] private SOEventVoid pauseEvent;
    [SerializeField] private SOEventVoid canInteractStartEvent;
    [SerializeField] private SOEventVoid canInteractStopEvent;
    
    [Header("GO with Handlers")]
    [SerializeField] private UIManager uiManager;

    
    void OnEnable()
    {
        if (pauseEvent)
        {
            pauseEvent.OnEventRaised -= uiManager.PauseMenuHandler;
            pauseEvent.OnEventRaised += uiManager.PauseMenuHandler;
        }

        if (canInteractStartEvent)
        {
            canInteractStartEvent.OnEventRaised -= uiManager.HUDInteractableHandler;
            canInteractStartEvent.OnEventRaised += uiManager.HUDInteractableHandler;
        }

        if (canInteractStopEvent)
        {
            canInteractStopEvent.OnEventRaised -= uiManager.HUDInteractableHandler;
            canInteractStopEvent.OnEventRaised += uiManager.HUDInteractableHandler;
        }
    }
    
    
    void OnDisable()
    {
        if (pauseEvent) pauseEvent.OnEventRaised -= uiManager.PauseMenuHandler;
        if (canInteractStartEvent) canInteractStartEvent.OnEventRaised -= uiManager.HUDInteractableHandler;
        if (canInteractStopEvent) canInteractStopEvent.OnEventRaised -= uiManager.HUDInteractableHandler;
    }
}
