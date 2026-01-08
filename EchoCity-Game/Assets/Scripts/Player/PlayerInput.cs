using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoCity
{
    public class PlayerInput : MonoBehaviour, IEventSender
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset inputActionAsset;
        private InputActionMap _playerActionMap;
        [SerializeField] private StarterAssetsInputs starterAssetsInputs;
        [SerializeField] private PlayerController playerController;

        [Header("Invoking Events")]
        [SerializeField] private SOSwitchToGameStateEvent switchToGameStateEvent;
        [SerializeField] private SOShowUIEvent showUIEvent;
        [SerializeField] private SOShowInteractionEvent showInteractionEvent;
        [SerializeField] private SOSetMaterialEvent setMaterialEvent;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => false;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Player };

        [Header("Observed Events")]
        [SerializeField] private SOPlayerInputEvent playerInputEvent;

        [Header("Interaction")]
        [SerializeField] private List<AreaInteractable> inRangeInteractables;
        [SerializeField] private AreaInteractable closestAreaInteractable;
        private bool _isShowingAreaDescription = false;
        private bool _isShowingDescription = false;
        [SerializeField] private float raycastDistance;

        // [Header("Test Parameters")]

        void Awake()
        {
            inRangeInteractables = new List<AreaInteractable>();
            _playerActionMap = inputActionAsset.FindActionMap("Player");
            if (_playerActionMap != null)
            {
                // Movement
                _playerActionMap["Move"].performed += OnMove;
                _playerActionMap["Move"].canceled += OnMove;
                _playerActionMap["Look"].performed += OnLook;
                _playerActionMap["Look"].canceled += OnLook;
                _playerActionMap["Jump"].performed += OnJump;
                _playerActionMap["Sprint"].performed += OnSprint;
                _playerActionMap["Interact"].performed += OnInteract;
                _playerActionMap["AreaInteract"].performed += OnAreaInteract;
                _playerActionMap["DropItem"].performed += OnDropItem;
                _playerActionMap["UseTool"].performed += OnUseTool;

                // UI
                _playerActionMap["OpenInventory"].started += OnOpenInventory;
                _playerActionMap["OpenInventory"].performed += OnCloseInventory;
                _playerActionMap["EnterPause"].performed += OnEnterPause;

                //Test
                //_playerActionMap["WearEcholocator"].performed += OnWearEcholocator;
                _playerActionMap["PlayerHit"].performed += OnPlayerHit;
                _playerActionMap["Test1"].performed += OnTest1;
                _playerActionMap["Test2"].performed += OnTest2;
                _playerActionMap["Test3"].performed += OnTest3;
                _playerActionMap["Test4"].performed += OnTest4;
            }

            //Error Logs
            Debug.Assert(inputActionAsset != null, "PlayerInput requires an InputActionAsset reference.");
            Debug.Assert(starterAssetsInputs != null, "PlayerInput requires a StarterAssetsInputs component reference.");
            Debug.Assert(playerController != null, "PlayerInput requires a PlayerController component reference.");
        }

        void OnEnable() => SubscribeToEvents();
        void OnDisable() => UnsubscribeFromEvents();

        private void SubscribeToEvents()
        {
            if (playerInputEvent) playerInputEvent.OnEventRaised += PlayerInputHandler;
        }
        private void UnsubscribeFromEvents()
        {
            if (playerInputEvent) playerInputEvent.OnEventRaised -= PlayerInputHandler;
        }

        private bool IsTargetVisible(Transform target) //BUG
        {
            Vector3 startPoint = Camera.main.transform.position;
            Vector3 direction = Camera.main.transform.forward;
            Ray ray = new Ray(startPoint, direction);
            // Raycast on all layers, ignore triggers to check only solid colliders (walls block interaction)
            // Debug.DrawRay(startPoint, direction * raycastDistance, Color.blue, 4f);
            int layerMask = ~((1 << 9) | (1 << 2)); // Ignore Player layer
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance + 0.5f, layerMask, QueryTriggerInteraction.Collide))
            {
                if (hit.transform == target || hit.transform.IsChildOf(target))
                    return true;
            }

            return false;
        }

        void Update()
        {
            #region raycast always active
            var origin = Camera.main.transform.position;
            var direction = Camera.main.transform.forward;
            Ray ray = new Ray(origin, direction);
            // Debug.DrawRay(origin, direction * raycastDistance, Color.yellow, 4f);
            Physics.Raycast(ray, out RaycastHit hitInfo, raycastDistance + 0.5f, ~((1 << 2) | (1 << 10)), QueryTriggerInteraction.Collide);
            if (hitInfo.collider != null)
            {
                var description = hitInfo.collider?.GetComponent<IHasDescription>();
                if (description != null && description.HasRaycastDescription && EcholocationVisibility.IsRevealedByAudio(hitInfo))
                {
                    if (!_isShowingDescription)
                    {
                        showInteractionEvent.RaiseEvent(this, description.IsInteractable, true, description.Description);
                        _isShowingDescription = true;
                    }
                }
                else if (_isShowingDescription)
                {
                    showInteractionEvent.RaiseEvent(this, false, false, null);
                    _isShowingDescription = false;
                }
            }
            else
            {
                if (_isShowingDescription)
                {
                    showInteractionEvent.RaiseEvent(this, false, false, null);
                    _isShowingDescription = false;
                }
            }
            #endregion
            #region area interactables check
            if (!_isShowingDescription)
            {
                if (inRangeInteractables.Count != 0)
                {
                    float closestDistance = Vector3.Distance(origin, closestAreaInteractable.AreaCenter);
                    foreach (var areaInteractable in inRangeInteractables)
                    {
                        float distance = Vector3.Distance(origin, areaInteractable.AreaCenter);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestAreaInteractable = areaInteractable;
                        }
                    }
                    _isShowingAreaDescription = true;
                    showInteractionEvent.RaiseEvent(this, false, true, closestAreaInteractable.Description);
                }
                else
                {
                    if (_isShowingAreaDescription)
                    {
                        showInteractionEvent.RaiseEvent(this, false, false, null);
                        _isShowingAreaDescription = false;
                    }
                }
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

        private void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var origin = Camera.main.transform.position;
                var direction = Camera.main.transform.forward;
                Ray ray = new Ray(origin, direction);
                Debug.DrawRay(origin, direction * raycastDistance, Color.red, 4f);
                if (_isShowingDescription)
                {
                    Physics.Raycast(ray, out RaycastHit hitInfo, raycastDistance, (1 << 6) | (1 << 9), QueryTriggerInteraction.Collide);
                    var pickable = hitInfo.collider?.GetComponent<Pickable>();
                    if (pickable != null && EcholocationVisibility.IsRevealedByAudio(hitInfo))
                    {
                        Log.DLazy(() => $"Interacting with Pickable: {pickable.name}", this);
                        if (pickable.PickableData == null || playerController.playerInventory.AddItem(pickable.PickableData, pickable.PickableData.Prefab))
                            pickable.Interact();
                        else playerController.EmitFullInventorySound();
                    }
                    else
                    {
                        var interactable = hitInfo.collider?.GetComponent<PlainInteractable>();
                        if (interactable != null && EcholocationVisibility.IsRevealedByAudio(hitInfo))
                            interactable.Interact();

                    }
                }
            }
        }

        private void OnAreaInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (inRangeInteractables.Count > 0)
            {
                Log.DLazy(() => $"Interacting with AreaInteractable: {closestAreaInteractable.name}", this);
                closestAreaInteractable.Interact();
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

        #region UI
        private void OnEnterPause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            switchToGameStateEvent.RaiseEvent(this, GameStatesEnum.Pause, null);
        }

        private void OnOpenInventory(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            _playerActionMap["Look"].performed -= OnLook;
            _playerActionMap["Interact"].performed -= OnInteract;
            _playerActionMap["DropItem"].performed -= OnDropItem;
            _playerActionMap["UseTool"].performed -= OnUseTool;

            switchToGameStateEvent.RaiseEvent(this, GameStatesEnum.Hud, new ToHUDStateParams(HudEnum.Inventory));

        }

        private void OnCloseInventory(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            switchToGameStateEvent.RaiseEvent(this, GameStatesEnum.Playing, null);
            _playerActionMap["Look"].performed += OnLook;
            _playerActionMap["Interact"].performed += OnInteract;
            _playerActionMap["DropItem"].performed += OnDropItem;
            _playerActionMap["UseTool"].performed += OnUseTool;
        }
        #endregion

        #region Test Input Actions
        private void OnWearEcholocator(InputAction.CallbackContext context)
        {
            if (context.performed)
                setMaterialEvent?.RaiseEvent(this, EchoMaterialCodeEnum.Toggle);
        }


        private void OnTest1(InputAction.CallbackContext context)
        {
            //DEATH Menu TEST
            if (context.performed)
            {
                switchToGameStateEvent?.RaiseEvent(this, GameStatesEnum.Death, null);
            }
        }

        private void OnTest2(InputAction.CallbackContext context)
        {
            //WARNING TEST
            if (context.performed)
            {
                showUIEvent?.RaiseEvent(this, ShowableUIEnum.Warning, new WarningParams("This is a test warning message!", Color.red));
            }
        }

        private void OnTest3(InputAction.CallbackContext context)
        {
            //WIN Menu TEST
            if (context.performed)
            {
                switchToGameStateEvent?.RaiseEvent(this, GameStatesEnum.Win, null);
            }
        }

        private void OnPlayerHit(InputAction.CallbackContext context)
        {
            //PLAYER HIT TEST
            if (context.performed)
            {
                playerController.TakeDamage(10f);
            }
        }

        private void OnTest4(InputAction.CallbackContext context) { }
        #endregion

        private void PlayerInputHandler(IEventSender sender, InputEnum input, bool activate)
        {
            if (input != InputEnum.Player) return;
            if (activate)
            {
                _playerActionMap.Enable();
                MethodsUI.HideCursor();
            }
            else
            {
                _playerActionMap.Disable();
            }
        }

        public void AddAreaInteractable(AreaInteractable interactable)
        {
            if (closestAreaInteractable == null)
                closestAreaInteractable = interactable;
            if (!inRangeInteractables.Contains(interactable))
                inRangeInteractables.Add(interactable);
        }

        public void RemoveAreaInteractable(AreaInteractable interactable)
        {
            if (inRangeInteractables.Contains(interactable))
                inRangeInteractables.Remove(interactable);
        }
    }
}
