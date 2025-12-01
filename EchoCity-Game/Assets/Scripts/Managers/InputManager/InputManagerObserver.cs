using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputManager))]
public class InputManagerObserver : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [Header("Observed Events")]
    [SerializeField] private SOAreaInteractableEvent enterInteractableAreaEvent;
    [SerializeField] private SOAreaInteractableEvent exitInteractableAreaEvent;
    [SerializeField] private SOEventVoid switchToPlayerActionMapEvent;



    [Header("GO with Handlers")]
    [SerializeField] private InputManager inputManager;

    void Awake()
    {
        TryGetComponent(out inputManager);
        TryGetComponent(out playerInput);
    }

    void OnEnable()
    {
        if (playerInput)
        {
            playerInput.onActionTriggered -= inputManager.HandleInput;
            playerInput.onActionTriggered += inputManager.HandleInput;
        }
        if (enterInteractableAreaEvent)
        {
            enterInteractableAreaEvent.OnEventRaised -= inputManager.EnterInteractionRangeHandler;
            enterInteractableAreaEvent.OnEventRaised += inputManager.EnterInteractionRangeHandler;
        }
        if (exitInteractableAreaEvent)
        {
            exitInteractableAreaEvent.OnEventRaised -= inputManager.ExitInteractionRangeHandler;
            exitInteractableAreaEvent.OnEventRaised += inputManager.ExitInteractionRangeHandler;
        }
        if (switchToPlayerActionMapEvent)
        {
            switchToPlayerActionMapEvent.OnEventRaised -= inputManager.SwitchToPlayerActionMapHandler;
            switchToPlayerActionMapEvent.OnEventRaised += inputManager.SwitchToPlayerActionMapHandler;
        }
    }

    void OnDisable()
    {
        if (playerInput) playerInput.onActionTriggered -= inputManager.HandleInput;
        if (enterInteractableAreaEvent) enterInteractableAreaEvent.OnEventRaised -= inputManager.EnterInteractionRangeHandler;
        if (exitInteractableAreaEvent) exitInteractableAreaEvent.OnEventRaised -= inputManager.ExitInteractionRangeHandler;
        if (switchToPlayerActionMapEvent) switchToPlayerActionMapEvent.OnEventRaised -= inputManager.SwitchToPlayerActionMapHandler;
    }



}
