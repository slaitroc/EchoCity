using EchoCity;
using UnityEngine;

public class UIManagerObserver : MonoBehaviour
{
    [Header("Observed Events")]
    [SerializeField] private SOEventVoid pauseMenuEvent;
    [SerializeField] private SOEventVoid canInteractStartEvent;
    [SerializeField] private SOEventVoid canInteractStopEvent;
    [SerializeField] private SOEventVoid openRadialMenuEvent;
    [SerializeField] private SOEventVoid closeRadialMenuEvent;
    [SerializeField] private SOPickableDataGameObjectEvent addInventoryItemEvent;
    // [SerializeField] private SOPickableDataEvent removeInventoryItemEvent;
    [SerializeField] private SOEventVoid rebuildRadialMenuEvent;


    [Header("GO with Handlers")]
    [SerializeField] private UIManager uiManager;


    void OnEnable()
    {
        if (pauseMenuEvent)
        {
            pauseMenuEvent.OnEventRaised -= uiManager.PauseMenuHandler;
            pauseMenuEvent.OnEventRaised += uiManager.PauseMenuHandler;
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
            openRadialMenuEvent.OnEventRaised -= uiManager.OpenRadialMenuHandler;
            openRadialMenuEvent.OnEventRaised += uiManager.OpenRadialMenuHandler;
        }

        if (closeRadialMenuEvent)
        {
            closeRadialMenuEvent.OnEventRaised -= uiManager.CloseRadialMenuHandler;
            closeRadialMenuEvent.OnEventRaised += uiManager.CloseRadialMenuHandler;
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
        if (pauseMenuEvent) pauseMenuEvent.OnEventRaised -= uiManager.PauseMenuHandler;
        if (canInteractStartEvent) canInteractStartEvent.OnEventRaised -= uiManager.HUDInteractableHandler;
        if (canInteractStopEvent) canInteractStopEvent.OnEventRaised -= uiManager.HUDInteractableHandler;
        if (openRadialMenuEvent) openRadialMenuEvent.OnEventRaised -= uiManager.OpenRadialMenuHandler;
        if (closeRadialMenuEvent) closeRadialMenuEvent.OnEventRaised -= uiManager.CloseRadialMenuHandler;
        // if (removeInventoryItemEvent) removeInventoryItemEvent.OnEventRaised -= uiManager.RemoveInventoryItemHandler;
        if (rebuildRadialMenuEvent) rebuildRadialMenuEvent.OnEventRaised -= uiManager.RebuildRadialMenuHandler;
    }
}
