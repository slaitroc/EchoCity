using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputManager : MonoBehaviour
{
#pragma warning disable CS0414
    #region Constants
    private string _LOG_TAG = "INPUT MANAGER";
    private string _LOG_COLOR = "#7039e8ff";
    #endregion
#pragma warning restore CS0414

    #region Serialized Fields
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputActionMap _playerActionMap;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [Header("Invoking Events")]
    [SerializeField] private SOEventVoid areaInteractionEvent;
    [SerializeField] private SOEventVoid pauseEvent;
    [SerializeField] private SOEventVoid canInteractStartEvent;
    [SerializeField] private SOEventVoid canInteractStopEvent;
    [SerializeField] private SOEventVoid materialToggleEvent;
    [SerializeField] private SOEventVoid openRadialMenuEvent;
    [SerializeField] private SOEventVoid closeRadialMenuEvent;

    [Header("Observed Events")]
    [SerializeField] private SOAreaInteractableEvent enterInteractableAreaEvent;
    [SerializeField] private SOAreaInteractableEvent exitInteractableAreaEvent;
    [SerializeField] private SOEventVoid enablePlayerActionMapEvent;
    [SerializeField] private SOEventVoid disablePlayerActionMapEvent;





    [Header("Interaction Range Colliders")]
    [SerializeField] private bool inInteractionRange = false;
    [SerializeField] private Interactable inRangeInteractable;
    #endregion
    #region Private Fields
    private const string UI_ACTION_MAP = "UI";
    private const string PLAYER_ACTION_MAP = "Player";
    private bool _canInteract;
    private bool _activeRenderer = true; // 0/false = PC Renderer, 1/true = Audio Visual
    private bool _isLookLocked;


    #endregion

    void Awake()
    {
        gameObject.tag = "InputManager";
        starterAssetsInputs = GameObject.FindGameObjectWithTag("Player")?.GetComponent<StarterAssetsInputs>();
        _playerActionMap = inputActionAsset.FindActionMap("Player");
        if (_playerActionMap != null)
        {
            _playerActionMap["Move"].performed += OnMove;
            _playerActionMap["Move"].canceled += OnMove;
            _playerActionMap["Look"].performed += OnLook;
            _playerActionMap["Look"].canceled += OnLook;
            _playerActionMap["Jump"].performed += OnJump;
            _playerActionMap["Sprint"].performed += OnSprint;
            _playerActionMap["WearEcholocator"].performed += OnWearEcholocator;
            _playerActionMap["Interact"].performed += OnInteract;
            _playerActionMap["UIRadialMenu"].performed += OnUIRadialMenu;
            _playerActionMap["UIRadialMenu"].canceled += OnUIRadialMenu;
            _playerActionMap["EnterPause"].performed += OnEnterPause;
        }

        _playerActionMap.Enable();
        MethodsUI.HideCursor();

        //Error Logs
        if (inputActionAsset == null)
            Log.E("InputActionAsset reference is missing", _LOG_COLOR, _LOG_TAG);
        if (starterAssetsInputs == null)
            Log.E("StarterAssetsInputs component not found on Player GameObject", _LOG_COLOR, _LOG_TAG);
    }

    void OnEnable() => SubscribeToEvents();
    void OnDisable() => UnsubscribeFromEvents();

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


    private void OnMove(InputAction.CallbackContext context)
    {
        if (starterAssetsInputs == null) return;
        // Always forward the current Vector2 value. This works for passthrough and
        // button interactions and avoids missing inputs due to phase checking.
        starterAssetsInputs.MoveInput(context.ReadValue<Vector2>());
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        if (starterAssetsInputs == null) return;

        // Forward current look value unless look is locked. Some devices/actions
        // may not deliver a Performed phase the way we expect; reading the value
        // directly is more consistent across bindings.
        var look = context.ReadValue<Vector2>();
        starterAssetsInputs.LookInput(_isLookLocked ? Vector2.zero : look);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (starterAssetsInputs == null) return;
        if (context.performed)
            starterAssetsInputs.JumpInput(context.performed);
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        if (starterAssetsInputs == null) return;
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
        if (!context.performed) return;
        if (inInteractionRange && inRangeInteractable != null)
            areaInteractionEvent.RaiseEvent();

    }

    private void OnEnterPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        pauseEvent?.RaiseEvent();

    }



    private void OnUIRadialMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            openRadialMenuEvent?.RaiseEvent();
            //enableUIActionMapEvent?.RaiseEvent();
        }

        if (context.canceled)
        {
            closeRadialMenuEvent?.RaiseEvent();
            //disableUIActionMapEvent?.RaiseEvent();
        }
    }

    private void EnablePlayerActionMap()
    {
        _playerActionMap.Enable();
        MethodsUI.HideCursor();
    }

    private void DisablePlayerActionMap()
    {
        _playerActionMap.Disable();
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

    private void SubscribeToEvents()
    {
        if (enterInteractableAreaEvent)
        {
            enterInteractableAreaEvent.OnEventRaised -= EnterInteractionRangeHandler;
            enterInteractableAreaEvent.OnEventRaised += EnterInteractionRangeHandler;
        }
        if (exitInteractableAreaEvent)
        {
            exitInteractableAreaEvent.OnEventRaised -= ExitInteractionRangeHandler;
            exitInteractableAreaEvent.OnEventRaised += ExitInteractionRangeHandler;
        }
        if (enablePlayerActionMapEvent)
        {
            enablePlayerActionMapEvent.OnEventRaised -= EnablePlayerActionMap;
            enablePlayerActionMapEvent.OnEventRaised += EnablePlayerActionMap;
        }
        if (disablePlayerActionMapEvent)
        {
            disablePlayerActionMapEvent.OnEventRaised -= DisablePlayerActionMap;
            disablePlayerActionMapEvent.OnEventRaised += DisablePlayerActionMap;
        }
    }
    private void UnsubscribeFromEvents()
    {
        if (enterInteractableAreaEvent) enterInteractableAreaEvent.OnEventRaised -= EnterInteractionRangeHandler;
        if (exitInteractableAreaEvent) exitInteractableAreaEvent.OnEventRaised -= ExitInteractionRangeHandler;
        if (enablePlayerActionMapEvent) enablePlayerActionMapEvent.OnEventRaised -= EnablePlayerActionMap;
        if (disablePlayerActionMapEvent) disablePlayerActionMapEvent.OnEventRaised -= DisablePlayerActionMap;
    }
}
