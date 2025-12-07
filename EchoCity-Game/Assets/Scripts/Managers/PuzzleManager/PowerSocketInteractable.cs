using UnityEngine;

namespace EchoCity
{
    /// <summary>
    /// Interactable component for power sockets in the bunker puzzle.
    /// Only the correct socket can be plugged in with the cable.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PowerSocketInteractable : Interactable
    {
        #region Constants
        protected override string _TYPE_LOG_TAG => "POWER_SOCKET";
        protected override string _LOG_TAG => isCorrectSocket ? "CORRECT_SOCKET" : "INCORRECT_SOCKET";
        #endregion

        #region Serialized Fields

        [Header("Socket Settings")]
        [Tooltip("Whether this is the correct socket that accepts the cable")]
        [SerializeField] private bool isCorrectSocket = false;

        [Tooltip("Mesh object representing the plugged-in cable (initially disabled)")]
        [SerializeField] private GameObject pluggedCableMesh;

        [Header("References")]
        [SerializeField] private BunkerPuzzleController puzzleController;

        [Header("Invoking Events")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;

        [Header("Sounds")]
        [SerializeField] private SOSoundSource plugInSound;
        [SerializeField] private SOSoundSource incorrectFeedbackSound;
        [SerializeField] private SOSoundSource needCableSound;

        #endregion

        #region Private Fields

        private PlayerInventory playerInventory;
        private bool isCablePlugged = false;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            if (puzzleController == null)
            {
                puzzleController = FindFirstObjectByType<BunkerPuzzleController>();
                if (puzzleController == null)
                {
                    Log.E($"BunkerPuzzleController not found in scene for {gameObject.name}", _LOG_COLOR, _LOG_TAG_FULL);
                }
            }

            // Find player inventory
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<PlayerInventory>();
            }

            // Initialize cable mesh state
            if (pluggedCableMesh != null)
            {
                pluggedCableMesh.SetActive(false);
            }
        }

        #endregion


        #region Interactable Implementation

        public override void Interact()
        {
            if (isCablePlugged)
            {
                Log.D("Cable already plugged in", _LOG_COLOR, _LOG_TAG_FULL);
                return;
            }

            if (!isCorrectSocket)
            {
                // Show incorrect feedback
                PlayIncorrectFeedback();
                return;
            }

            // Check if player has cable
            if (!HasCableInInventory())
            {
                PlayNeedCableFeedback();
                return;
            }

            // Check if cable is already plugged (via puzzle controller)
            if (puzzleController != null && puzzleController.CablePlugged)
            {
                Log.D("Cable already plugged into socket", _LOG_COLOR, _LOG_TAG_FULL);
                return;
            }

            // Plug in the cable
            PlugInCable();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if the player has the cable in their inventory
        /// </summary>
        private bool HasCableInInventory()
        {
            if (playerInventory == null)
            {
                Log.W("PlayerInventory not found", _LOG_COLOR, _LOG_TAG_FULL);
                return false;
            }

            // Check if puzzle controller knows about the cable
            if (puzzleController != null && puzzleController.HasCable)
            {
                return true;
            }

            // Also check inventory items by name (backup method)
            foreach (var item in playerInventory.Items)
            {
                if (item != null && item.Data.Name == "Loose Cable")
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Plugs in the cable and notifies the puzzle controller
        /// </summary>
        private void PlugInCable()
        {
            isCablePlugged = true;

            // Show visual feedback
            if (pluggedCableMesh != null)
            {
                pluggedCableMesh.SetActive(true);
            }

            // Play sound
            if (plugInSound != null)
            {
                ECSound.PlayAtPosition(plugInSound, transform.position, newAudioSphereEvent, "SFX");
            }

            // Notify puzzle controller
            if (puzzleController != null)
            {
                puzzleController.OnCablePluggedIn();
            }

            Log.D("Cable plugged into socket", _LOG_COLOR, _LOG_TAG_FULL);
        }

        /// <summary>
        /// Plays feedback sound for incorrect socket
        /// </summary>
        private void PlayIncorrectFeedback()
        {
            if (incorrectFeedbackSound != null)
            {
                ECSound.PlayAtPosition(incorrectFeedbackSound, transform.position, newAudioSphereEvent, "SFX");
            }
            Log.D("Incorrect socket - this one doesn't work", _LOG_COLOR, _LOG_TAG_FULL);
        }

        /// <summary>
        /// Plays feedback sound when player needs cable
        /// </summary>
        private void PlayNeedCableFeedback()
        {
            if (needCableSound != null)
            {
                ECSound.PlayAtPosition(needCableSound, transform.position, newAudioSphereEvent, "SFX");
            }
            Log.D("Player needs cable to plug in", _LOG_COLOR, _LOG_TAG_FULL);
        }

        #endregion
    }
}

