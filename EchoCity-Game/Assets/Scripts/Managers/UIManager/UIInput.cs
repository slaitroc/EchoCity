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

        [SerializeField] private InputActionAsset inputActionAsset;

        [Header("Invoking Events")]
        [SerializeField] private SOEventVoid pauseGameEvent;
        [SerializeField] private SOEventVoid pauseMenuEvent;
        [SerializeField] private SOEventVoid enablePlayerActionMapEvent;


        [Header("Observing Events")]
        [SerializeField] private SOEventVoid enableUIActionMapEvent;

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
            pauseMenuEvent?.RaiseEvent();
            pauseGameEvent?.RaiseEvent();

        }

        public void EnableUIActionMap() => _uiActionMap.Enable();
        public void DisableUIActionMap() => _uiActionMap.Disable();
    }
}