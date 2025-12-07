using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoCity
{
    public class PlayerInput : MonoBehaviour
    {
#pragma warning disable CS0414
        #region Constants
        private string _LOG_TAG = "PLAYER INPUT";
        private string _LOG_COLOR = "#7039e8ff";
        #endregion
#pragma warning restore CS0414

        [Header("Input")]
        [SerializeField] private InputActionAsset inputActionAsset;
        private InputActionMap _playerActionMap;
        [SerializeField] private StarterAssetsInputs starterAssetsInputs;
        [SerializeField] private PlayerController playerController;

        [Header("Invoking Events")]
        [SerializeField] private SOEventVoid switchToPauseStateEvent;
        [SerializeField] private SOEventVoid switchToPlayingStateEvent;
        [SerializeField] private SOHudEnumEvent switchToHudStateEvent;
        [SerializeField] private SOEventVoid canInteractStartEvent;
        [SerializeField] private SOEventVoid canInteractStopEvent;
        [SerializeField] private SOEventVoid materialToggleEvent;
        [SerializeField] private SOEventVoid areaInteractionEvent;

        [Header("Observed Events")]
        [SerializeField] private SOAreaInteractableEvent enterInteractableAreaEvent;
        [SerializeField] private SOAreaInteractableEvent exitInteractableAreaEvent;
        [SerializeField] private SOEventVoid enablePlayerActionMapEvent;
        [SerializeField] private SOEventVoid disablePlayerActionMapEvent;
        [SerializeField] private SOEventVoid wearEcholocatorEvent;

        [Header("Interaction Range Colliders")]
        [SerializeField] private bool inInteractionRange = false;
        [SerializeField] private Interactable inRangeInteractable;


        [Header("Test Events")]
        [Header("Invoking")]
        [SerializeField] private SOIntegerPickableDataGameObjectEvent itemEquippedEvent;
        [SerializeField] private SOPickable examplePickable;
        [SerializeField] private SOEnemyAIEvent playerHitEvent;
        [SerializeField] private SOStringColorEvent spawnWarningEvent;
        [SerializeField] private SOEventVoid switchToDeathStateEvent;
        [SerializeField] private SOEventVoid switchToWinStateEvent;

        [Header("Dialog")]
        [SerializeField] private SODialogDataEvent switchToNarrationStateEvent;
        [SerializeField] private SODialogContainer exampleDialogData;

        private bool _canInteract;


        void Awake()
        {
            //starterAssetsInputs = GameObject.FindGameObjectWithTag("Player")?.GetComponent<StarterAssetsInputs>();
            _playerActionMap = inputActionAsset.FindActionMap("Player");
            if (_playerActionMap != null)
            {
                _playerActionMap["Move"].performed += OnMove;
                _playerActionMap["Move"].canceled += OnMove;
                _playerActionMap["Look"].performed += OnLook;
                _playerActionMap["Look"].canceled += OnLook;
                _playerActionMap["Jump"].performed += OnJump;
                _playerActionMap["Sprint"].performed += OnSprint;
                //_playerActionMap["WearEcholocator"].performed += OnWearEcholocator;
                _playerActionMap["Interact"].performed += OnInteract;
                _playerActionMap["OpenInventory"].started += OnOpenInventory;
                _playerActionMap["OpenInventory"].performed += OnCloseInventory;
                _playerActionMap["EnterPause"].performed += OnEnterPause;
                _playerActionMap["DropItem"].performed += OnDropItem;
                _playerActionMap["UseTool"].performed += OnUseTool;
                _playerActionMap["PlayerHit"].performed += OnPlayerHit;

                _playerActionMap["Test1"].performed += OnTest1;
                _playerActionMap["Test2"].performed += OnTest2;
                _playerActionMap["Test3"].performed += OnTest3;
                _playerActionMap["Test4"].performed += OnTest4;

            }

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
            starterAssetsInputs.LookInput(look);
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
                // materialToggleEvent?.RaiseEvent();
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
            switchToPauseStateEvent.RaiseEvent();
        }


        private void OnOpenInventory(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            _playerActionMap["Look"].performed -= OnLook;
            switchToHudStateEvent.RaiseEvent(HudEnum.Inventory);

        }

        private void OnCloseInventory(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            switchToPlayingStateEvent.RaiseEvent();
            _playerActionMap["Look"].performed += OnLook;
        }

        private void OnTest1(InputAction.CallbackContext context)
        {
            //TESTS HERE
            if (context.performed)
            {
                switchToDeathStateEvent?.RaiseEvent();
            }
        }

        private void OnTest2(InputAction.CallbackContext context)
        {
            //EQUIP ITEM TEST
            if (context.performed)
            {
                spawnWarningEvent?.RaiseEvent("Warning: Enemy Approaching!", Color.red);
            }
        }

        private void OnTest3(InputAction.CallbackContext context)
        {
            //EQUIP ITEM TEST
            if (context.performed)
            {
                switchToWinStateEvent?.RaiseEvent();
            }
        }

        private void OnPlayerHit(InputAction.CallbackContext context)
        {
            //PLAYER HIT TEST
            if (context.performed)
            {
                playerHitEvent?.RaiseEvent(null);
            }
        }


        private void OnDropItem(InputAction.CallbackContext context)
        {
            //DROP ITEM TEST
            if (context.performed)
                playerController.DropItem();
        }

        private void OnUseTool(InputAction.CallbackContext context)
        {
            //USE TOOL TEST
            if (context.performed)
            {
                playerController.UseTool();
            }
        }

        private void OnTest4(InputAction.CallbackContext context)
        {
            // SPAWN DIALOG TEST
            if (context.performed)
            {
                switchToNarrationStateEvent.RaiseEvent(new DialogData(exampleDialogData.DialogLines));
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

        private void WearEcholocatorHandler()
        {
            materialToggleEvent?.RaiseEvent();
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
            if (wearEcholocatorEvent)
            {
                wearEcholocatorEvent.OnEventRaised -= WearEcholocatorHandler;
                wearEcholocatorEvent.OnEventRaised += WearEcholocatorHandler;
            }
        }
        private void UnsubscribeFromEvents()
        {
            if (enterInteractableAreaEvent) enterInteractableAreaEvent.OnEventRaised -= EnterInteractionRangeHandler;
            if (exitInteractableAreaEvent) exitInteractableAreaEvent.OnEventRaised -= ExitInteractionRangeHandler;
            if (enablePlayerActionMapEvent) enablePlayerActionMapEvent.OnEventRaised -= EnablePlayerActionMap;
            if (disablePlayerActionMapEvent) disablePlayerActionMapEvent.OnEventRaised -= DisablePlayerActionMap;
            if (wearEcholocatorEvent) wearEcholocatorEvent.OnEventRaised -= WearEcholocatorHandler;
        }
    }
}
