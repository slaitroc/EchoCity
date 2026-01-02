using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoCity
{
    public class UIInput : MonoBehaviour
    {
#pragma warning disable CS0414
        private const string _LOG_TAG = "UI INPUT";
        private const string _LOG_COLOR = "#39e8d1ff";
#pragma warning restore CS0414

        [Header("UI")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private InputActionAsset inputActionAsset;


        [Header("Observing Events")]
        [SerializeField] private SOEventVoid enableUIActionMapEvent;
        [SerializeField] private SOEventVoid disableUIActionMapEvent;

        private InputActionMap _uiActionMap;

        void Awake()
        {
            InitializeUIBinding();
            _uiActionMap.Disable();


            //Error Logs
            if (inputActionAsset == null)
                Log.E("InputActionAsset is not assigned in UIInputManager", _LOG_COLOR, _LOG_TAG);
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
            enableUIActionMapEvent.OnEventRaised -= EnableUIActionMap;
            enableUIActionMapEvent.OnEventRaised += EnableUIActionMap;

            disableUIActionMapEvent.OnEventRaised -= DisableUIActionMap;
            disableUIActionMapEvent.OnEventRaised += DisableUIActionMap;
        }

        private void EnableUIActionMap(IEventSender sender) => _uiActionMap.Enable();
        private void DisableUIActionMap(IEventSender sender) => _uiActionMap.Disable();
    }
}