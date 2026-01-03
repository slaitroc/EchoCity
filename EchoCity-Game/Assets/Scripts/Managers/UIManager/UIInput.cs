using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoCity
{
    public class UIInput : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private InputActionAsset inputActionAsset;


        [Header("Observing Events")]
        [SerializeField] private SOPlayerInputEvent playerInputEvent;

        private InputActionMap _uiActionMap;

        void Awake()
        {
            InitializeUIBinding();
            _uiActionMap.Disable();


            //Error Logs
            if (inputActionAsset == null)
                Log.ELazy(() => $"Input Action Asset is not assigned!", this);
        }

        private void InitializeUIBinding()
        {
            if (inputActionAsset != null)
            {
                _uiActionMap = inputActionAsset.FindActionMap("UI");
                _uiActionMap["ExitPause"].performed += OnExitPause;
            }
        }

        private void OnExitPause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            uiManager.SwitchToPlayState();
        }

        private void OnEnable()
        {
            playerInputEvent.OnEventRaised += PlayerInputHandler;
        }

        private void OnDisable()
        {
            playerInputEvent.OnEventRaised -= PlayerInputHandler;
        }

        private void PlayerInputHandler(IEventSender sender, InputEnum input, bool activate)
        {
            if (input == InputEnum.UI)
                if (activate)
                    _uiActionMap.Enable();
                else
                    _uiActionMap.Disable();
        }
    }
}