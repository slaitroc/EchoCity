using System;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;


[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(InputManagerObserver))]
public class InputManager : MonoBehaviour
{
#pragma warning disable CS0414
    #region Constants
    private string _LOG_TAG = "INPUT MANAGER";
    private string _LOG_COLOR = "#7039e8ff";
    #endregion
#pragma warning restore CS0414

    #region Serialized Fields
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [Header("Invoking Events")]
    [SerializeField] private SOEventVoid areaInteractionEvent;
    [SerializeField] private SOEventVoid pauseEvent;
    [SerializeField] private SOEventVoid canInteractStartEvent;
    [SerializeField] private SOEventVoid canInteractStopEvent;
    [SerializeField] private SOEventVoid materialToggleEvent;
    [SerializeField] private SOEventVoid openRadialMenuEvent;
    [SerializeField] private SOEventVoid closeRadialMenuEvent;


    [Header("Interaction Range Colliders")]
    [SerializeField] private bool inInteractionRange = false;
    [SerializeField] private Interactable inRangeInteractable;
    #endregion
    #region Private Fields
    private const string UI_ACTION_MAP = "UI";
    private const string PLAYER_ACTION_MAP = "Player";
    private PlayerInput _playerInput;
    private bool _canInteract;
    private bool _activeRenderer = true; // 0/false = PC Renderer, 1/true = Audio Visual
    private bool _isRadialMenuOpen;
    
    
    #endregion

    void Awake()
    {
        gameObject.tag = "InputManager";
        starterAssetsInputs = GameObject.FindGameObjectWithTag("Player")?.GetComponent<StarterAssetsInputs>();
        if (starterAssetsInputs == null)
            Log.E("StarterAssetsInputs component not found on Player GameObject", _LOG_COLOR, _LOG_TAG);

        TryGetComponent(out _playerInput);
    }

    void Update()
    {
        #region raycast always active
        var origin = Camera.main.transform.position;
        var direction = Camera.main.transform.forward;
        Ray ray = new Ray(origin, direction);
        Physics.Raycast(ray, out RaycastHit hitInfo, 10f, 1 << 6, QueryTriggerInteraction.Collide);
        var interactable = hitInfo.collider?.GetComponent<Interactable>();
        if (interactable != null)
        {
            if (!_canInteract)
            {
                canInteractStartEvent.RaiseEvent();
                //Log.D("Can interact", _LOG_COLOR, _LOG_TAG);
                _canInteract = true;
            }
        }
        else if (_canInteract)
        {
            canInteractStopEvent.RaiseEvent();
            //Log.D("Can no longer interact", _LOG_COLOR, _LOG_TAG);
            _canInteract = false;
        }
        #endregion
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        if (context.action == null) return;

        if (context.action.name == "Move")
            OnMove(context);

        if (context.action.name == "Look")
            OnLook(context);

        if (context.action.name == "Jump")
            OnJump(context);

        if (context.action.name == "Sprint")
            OnSprint(context);

        if (context.action.name == "WearEcholocator")
            OnWearEcholocator(context);

        if (context.action.name == "Interact")
            OnInteract(context);

        if (context.action.name == "RangeInteract")
            OnAreaInteract(context);

        if (context.action.name == "EnterPause")
            OnEnterPause(context);

        if (context.action.name == "ExitPause")
            OnExitPause(context);

        if (context.action.name == "OpenRadialMenu")
        {

            OnOpenRadialMenu(context);
        }

    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            starterAssetsInputs.MoveInput(context.ReadValue<Vector2>());
        else if (context.canceled)
            starterAssetsInputs.MoveInput(Vector2.zero);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed && !_isRadialMenuOpen)
            starterAssetsInputs.LookInput(context.ReadValue<Vector2>());
        else if (context.canceled)
            starterAssetsInputs.LookInput(Vector2.zero);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            starterAssetsInputs.JumpInput(context.performed);
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // For passthrough/value bindings read the numeric value and treat >0.5 as pressed
            starterAssetsInputs.SprintInput(context.ReadValue<float>() > 0.5f);
        }
    }

    private void OnWearEcholocator(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            materialToggleEvent?.RaiseEvent();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var origin = Camera.main.transform.position;
            var direction = Camera.main.transform.forward;
            Ray ray = new Ray(origin, direction);
            Debug.DrawRay(origin, direction * 10f, Color.red, 4f);
            if (_canInteract)
            {
                Physics.Raycast(ray, out RaycastHit hitInfo, 10f, 1 << 6, QueryTriggerInteraction.Collide);
                var interactable = hitInfo.collider?.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }

    private void OnAreaInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inInteractionRange && inRangeInteractable != null)
                areaInteractionEvent.RaiseEvent();
        }
    }

    private void OnEnterPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerInput.SwitchCurrentActionMap(UI_ACTION_MAP);
            Log.D("Switched to Action Map: " + _playerInput.currentActionMap.ToString(), _LOG_COLOR, _LOG_TAG);
            pauseEvent?.RaiseEvent();
        }
    }

     private void OnExitPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerInput.SwitchCurrentActionMap(PLAYER_ACTION_MAP);
        }
    }

    private void OnOpenRadialMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            openRadialMenuEvent?.RaiseEvent();
            _isRadialMenuOpen = true;
        }
        else if (context.canceled)
        {
            closeRadialMenuEvent?.RaiseEvent();
            _isRadialMenuOpen = false;
        }
    }





    //DANGER distinct InteractableArea's colliders MUST NOT intersect otherwise this implementation WILL NOT WORK as expected!
    public void EnterInteractionRangeHandler(Interactable interactable)
    {
        inInteractionRange = true;
        inRangeInteractable = interactable;
    }
    public void ExitInteractionRangeHandler(Interactable interactable)
    {
        inInteractionRange = false;
        inRangeInteractable = null;
    }
    public void SwitchToPlayerActionMapHandler()
    { 
        _playerInput.SwitchCurrentActionMap(PLAYER_ACTION_MAP);
    }
}
