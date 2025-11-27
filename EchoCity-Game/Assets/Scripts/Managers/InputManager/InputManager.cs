using System;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;


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

    [Header("Interaction Range Colliders")]
    [SerializeField] private bool inInteractionRange = false;
    [SerializeField] private Interactable inRangeInteractable;
    #endregion
    #region Private Fields
    private bool _canInteract;
    private bool _activeRenderer = true; // 0/false = PC Renderer, 1/true = Audio Visual

    #endregion

    void Awake()
    {
        gameObject.tag = "InputManager";
        starterAssetsInputs = GameObject.FindGameObjectWithTag("Player")?.GetComponent<StarterAssetsInputs>();
        if (starterAssetsInputs == null)
            Log.E("StarterAssetsInputs component not found on Player GameObject", _LOG_COLOR, _LOG_TAG);
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

        if (context.action.name == "Pause")
            OnPause(context);
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
        if (context.performed)
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
            starterAssetsInputs.SprintInput(context.performed);
    }

    private void OnWearEcholocator(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _activeRenderer = !_activeRenderer;
            Camera.main.GetUniversalAdditionalCameraData().SetRenderer(Convert.ToInt32(!_activeRenderer));
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_canInteract)
            {
                var origin = Camera.main.transform.position;
                var direction = Camera.main.transform.forward;
                Ray ray = new Ray(origin, direction);
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

    private void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
            pauseEvent?.RaiseEvent();
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
}
