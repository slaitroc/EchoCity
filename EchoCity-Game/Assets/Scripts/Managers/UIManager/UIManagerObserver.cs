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
    [SerializeField] private SODialogDataEvent spawnDialogEvent;
    [SerializeField] private SOStringColorEvent spawnWarningEvent;
    [SerializeField] private SOEventVoid startGameEvent;


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

        if (rebuildRadialMenuEvent)
        {
            rebuildRadialMenuEvent.OnEventRaised -= uiManager.RebuildRadialMenuHandler;
            rebuildRadialMenuEvent.OnEventRaised += uiManager.RebuildRadialMenuHandler;
        }

        if (spawnDialogEvent)
        {
            spawnDialogEvent.OnEventRaised -= uiManager.SpawnDialogHandler;
            spawnDialogEvent.OnEventRaised += uiManager.SpawnDialogHandler;
        }

        if (spawnWarningEvent)
        {
            spawnWarningEvent.OnEventRaised -= uiManager.SpawnWarningHandler;
            spawnWarningEvent.OnEventRaised += uiManager.SpawnWarningHandler;
        }

        if (startGameEvent)
        {
            startGameEvent.OnEventRaised -= uiManager.StartGameHandler;
            startGameEvent.OnEventRaised += uiManager.StartGameHandler;
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
        if (spawnDialogEvent) spawnDialogEvent.OnEventRaised -= uiManager.SpawnDialogHandler;
        if (spawnWarningEvent) spawnWarningEvent.OnEventRaised -= uiManager.SpawnWarningHandler;   
        if (startGameEvent) startGameEvent.OnEventRaised -= uiManager.StartGameHandler;
    }
}
