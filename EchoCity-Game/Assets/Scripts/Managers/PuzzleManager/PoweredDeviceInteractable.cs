using UnityEngine;

namespace EchoCity
{
    /// <summary>
    /// Interactable component for devices that can be toggled on/off after power is restored.
    /// Examples: fans, lights, hum emitters, lab equipment.
    /// </summary>
    [RequireComponent(typeof(AudioEmitter))]
    public class PoweredDeviceInteractable : Interactable
    {
        #region Constants
        protected override string _TYPE_LOG_TAG => "POWERED_DEVICE";
        protected override string _LOG_TAG => gameObject.name;
        #endregion

        #region Serialized Fields

        [Header("Device Settings")]
        [Tooltip("Whether the device should start powered on when power is restored")]
        [SerializeField] private bool startPoweredOn = false;

        [Header("References")]
        [SerializeField] private BunkerPuzzleController puzzleController;
        [SerializeField] private AudioEmitter audioEmitter;
        [SerializeField] private Animator deviceAnimator;
        [SerializeField] private Light deviceLight;

        [Header("Invoking Events")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;

        [Header("Sounds")]
        [SerializeField] private SOSoundSource deviceOnSound;
        [SerializeField] private SOSoundSource deviceOffSound;

        #endregion

        #region Private Fields

        private bool isPoweredOn = false;
        private bool wasEnabledOnStart = false;
        private readonly int hashIsOn = Animator.StringToHash("isOn");

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // Get AudioEmitter component
            if (audioEmitter == null)
            {
                TryGetComponent(out audioEmitter);
            }

            // Get Animator if not assigned
            if (deviceAnimator == null)
            {
                TryGetComponent(out deviceAnimator);
            }

            // Get Light component if not assigned
            if (deviceLight == null)
            {
                TryGetComponent(out deviceLight);
            }

            // Find puzzle controller
            if (puzzleController == null)
            {
                puzzleController = FindObjectOfType<BunkerPuzzleController>();
            }

            // Store initial AudioEmitter state
            if (audioEmitter != null)
            {
                wasEnabledOnStart = audioEmitter.enabled;
            }

            // Initially disable device if power is not restored
            if (puzzleController != null && !puzzleController.CablePlugged)
            {
                SetDeviceState(false, false);
            }
        }

        #endregion


        #region Interactable Implementation

        public override void Interact()
        {
            // Check if power is restored
            if (puzzleController == null || !puzzleController.CablePlugged)
            {
                Log.D("Device has no power", _LOG_COLOR, _LOG_TAG_FULL);
                return;
            }

            // Toggle device state
            isPoweredOn = !isPoweredOn;
            SetDeviceState(isPoweredOn, true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called by puzzle controller when power is restored.
        /// Enables the device if it should start powered on.
        /// </summary>
        public void OnPowerRestored()
        {
            if (startPoweredOn)
            {
                isPoweredOn = true;
                SetDeviceState(true, false);
            }
            else
            {
                isPoweredOn = false;
                SetDeviceState(false, false);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the device's powered state and updates all related components
        /// </summary>
        /// <param name="isOn">Whether the device should be on</param>
        /// <param name="playSound">Whether to play toggle sound</param>
        private void SetDeviceState(bool isOn, bool playSound)
        {
            // Update AudioEmitter
            if (audioEmitter != null)
            {
                audioEmitter.enabled = isOn;
            }

            // Update Animator
            if (deviceAnimator != null)
            {
                deviceAnimator.SetBool(hashIsOn, isOn);
            }

            // Update Light
            if (deviceLight != null)
            {
                deviceLight.enabled = isOn;
            }

            // Play sound if requested
            if (playSound)
            {
                if (isOn && deviceOnSound != null)
                {
                    ECSound.PlaySoundAtPosition(deviceOnSound, transform.position, newAudioSphereEvent, "SFX");
                }
                else if (!isOn && deviceOffSound != null)
                {
                    ECSound.PlaySoundAtPosition(deviceOffSound, transform.position, newAudioSphereEvent, "SFX");
                }
            }

            Log.D($"Device {(isOn ? "turned on" : "turned off")}", _LOG_COLOR, _LOG_TAG_FULL);
        }

        #endregion
    }
}

