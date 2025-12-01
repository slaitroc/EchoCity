using EchoCity;
using UnityEngine;

public class UIManagerObserver : MonoBehaviour
{
    [Header("Observed Events")]
    [SerializeField] private SOEventVoid pauseEvent;
    [SerializeField] private SOEventVoid canInteractStartEvent;
    [SerializeField] private SOEventVoid canInteractStopEvent;
    [SerializeField] private SOEventVoid openRadialMenuEvent;
    [SerializeField] private SOEventVoid closeRadialMenuEvent;
    [SerializeField] private SOPickableDataEvent addInventoryItemEvent;
    // [SerializeField] private SOPickableDataEvent removeInventoryItemEvent;
    [SerializeField] private SOEventVoid rebuildRadialMenuEvent;
   
    
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

        if (openRadialMenuEvent)
        {
            openRadialMenuEvent.OnEventRaised -= uiManager.RadialMenuOpenHandler;
            openRadialMenuEvent.OnEventRaised += uiManager.RadialMenuOpenHandler;
        }

        if (closeRadialMenuEvent)
        {
            closeRadialMenuEvent.OnEventRaised -= uiManager.RadialMenuCloseHandler;
            closeRadialMenuEvent.OnEventRaised += uiManager.RadialMenuCloseHandler;
        }

        if (addInventoryItemEvent)
        {
            addInventoryItemEvent.OnEventRaised -= uiManager.AddInventoryItemHandler;
            addInventoryItemEvent.OnEventRaised += uiManager.AddInventoryItemHandler;
        }

        // if (removeInventoryItemEvent)
        // {
        //     removeInventoryItemEvent.OnEventRaised -= uiManager.RemoveInventoryItemHandler;
        //     removeInventoryItemEvent.OnEventRaised += uiManager.RemoveInventoryItemHandler;
        // }

        if (rebuildRadialMenuEvent)
        {
            rebuildRadialMenuEvent.OnEventRaised -= uiManager.RebuildRadialMenuHandler;
            rebuildRadialMenuEvent.OnEventRaised += uiManager.RebuildRadialMenuHandler;
        }
    }
    
    
    void OnDisable()
    {
        if (pauseEvent) pauseEvent.OnEventRaised -= uiManager.PauseMenuHandler;
        if (canInteractStartEvent) canInteractStartEvent.OnEventRaised -= uiManager.HUDInteractableHandler;
        if (canInteractStopEvent) canInteractStopEvent.OnEventRaised -= uiManager.HUDInteractableHandler;
        if (openRadialMenuEvent) openRadialMenuEvent.OnEventRaised -= uiManager.RadialMenuOpenHandler;
        if (closeRadialMenuEvent) closeRadialMenuEvent.OnEventRaised -= uiManager.RadialMenuCloseHandler;
        if (addInventoryItemEvent) addInventoryItemEvent.OnEventRaised -= uiManager.AddInventoryItemHandler;
        // if (removeInventoryItemEvent) removeInventoryItemEvent.OnEventRaised -= uiManager.RemoveInventoryItemHandler;
        if (rebuildRadialMenuEvent) rebuildRadialMenuEvent.OnEventRaised -= uiManager.RebuildRadialMenuHandler;
    }
}
