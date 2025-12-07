using UnityEngine;

namespace EchoCity
{
    /// <summary>
    /// Interactable component for floppy disk readers in the bunker puzzle.
    /// Only the correct reader can eject the floppy disk when power is restored.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class FloppyDiskReaderInteractable : Interactable
    {
        #region Constants
        protected override string _TYPE_LOG_TAG => "FLOPPY_READER";
        protected override string _LOG_TAG => isCorrectReader ? "CORRECT_READER" : "INCORRECT_READER";
        #endregion

        #region Serialized Fields

        [Header("Reader Settings")]
        [Tooltip("Whether this is the correct reader that ejects Kael's floppy disk")]
        [SerializeField] private bool isCorrectReader = false;

        [Header("References")]
        [SerializeField] private BunkerPuzzleController puzzleController;
        [SerializeField] private Animator readerAnimator;
        [SerializeField] private GameObject floppyDiskPrefab;
        [SerializeField] private Transform floppyDiskSpawnPoint;

        [Header("Invoking Events")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;

        [Header("Sounds")]
        [SerializeField] private SOSoundSource ejectSound;
        [SerializeField] private SOSoundSource noPowerSound;
        [SerializeField] private SOSoundSource notWorkingSound;

        #endregion

        #region Private Fields

        private bool hasEjected = false;
        private readonly int hashEject = Animator.StringToHash("Eject");

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

            // Get Animator if not assigned
            if (readerAnimator == null)
            {
                TryGetComponent(out readerAnimator);
            }

            // Validate spawn point
            if (floppyDiskSpawnPoint == null && isCorrectReader)
            {
                Log.W($"FloppyDiskSpawnPoint not assigned for {gameObject.name}. Using reader position.", _LOG_COLOR, _LOG_TAG_FULL);
                floppyDiskSpawnPoint = transform;
            }
        }

        #endregion


        #region Interactable Implementation

        public override void Interact()
        {
            if (!isCorrectReader)
            {
                // Show "Device not working" feedback
                PlayNotWorkingFeedback();
                return;
            }

            if (hasEjected)
            {
                Log.D("Floppy disk already ejected", _LOG_COLOR, _LOG_TAG_FULL);
                return;
            }

            if (puzzleController == null || !puzzleController.CablePlugged)
            {
                // No power
                PlayNoPowerFeedback();
                return;
            }

            // Eject floppy disk
            EjectFloppyDisk();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ejects the floppy disk from the reader
        /// </summary>
        private void EjectFloppyDisk()
        {
            hasEjected = true;

            // Play animation
            if (readerAnimator != null)
            {
                readerAnimator.SetTrigger(hashEject);
            }

            // Spawn floppy disk
            if (floppyDiskPrefab != null && floppyDiskSpawnPoint != null)
            {
                GameObject floppy = Instantiate(floppyDiskPrefab, floppyDiskSpawnPoint.position, floppyDiskSpawnPoint.rotation);
                Log.D($"Floppy disk spawned at {floppyDiskSpawnPoint.position}", _LOG_COLOR, _LOG_TAG_FULL);
            }
            else
            {
                Log.W($"Cannot spawn floppy disk - prefab or spawn point missing on {gameObject.name}", _LOG_COLOR, _LOG_TAG_FULL);
            }

            // Play sound
            if (ejectSound != null)
            {
                ECSound.PlayAtPosition(ejectSound, transform.position, newAudioSphereEvent, "SFX");
            }

            // Notify puzzle controller
            if (puzzleController != null)
            {
                puzzleController.OnFloppyEjected();
            }

            Log.D("Floppy disk ejected from reader", _LOG_COLOR, _LOG_TAG_FULL);
        }

        /// <summary>
        /// Plays feedback sound when device has no power
        /// </summary>
        private void PlayNoPowerFeedback()
        {
            if (noPowerSound != null)
            {
                ECSound.PlayAtPosition(noPowerSound, transform.position, newAudioSphereEvent, "SFX");
            }
            Log.D("No power - device cannot work", _LOG_COLOR, _LOG_TAG_FULL);
        }

        /// <summary>
        /// Plays feedback sound when device is not working
        /// </summary>
        private void PlayNotWorkingFeedback()
        {
            if (notWorkingSound != null)
            {
                ECSound.PlayAtPosition(notWorkingSound, transform.position, newAudioSphereEvent, "SFX");
            }
            Log.D("Device not working", _LOG_COLOR, _LOG_TAG_FULL);
        }

        #endregion
    }
}

