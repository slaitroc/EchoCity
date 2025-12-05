using System.Collections;
using UnityEngine;

namespace EchoCity
{
    /// <summary>
    /// Interactable component for the bunker exit door.
    /// Checks puzzle completion before allowing the player to force open the door.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class BunkerExitDoorInteractable : Interactable
    {
        #region Constants
        protected override string _TYPE_LOG_TAG => "EXIT_DOOR";
        protected override string _LOG_TAG => "BUNKER_EXIT";
        #endregion

        #region Serialized Fields

        [Header("References")]
        [SerializeField] private BunkerPuzzleController puzzleController;
        [SerializeField] private Animator doorAnimator;
        [SerializeField] private SceneLoader sceneLoader;

        [Header("Invoking Events")]
        [SerializeField] private SOSoundEmissionDataEvent newAudioSphereEvent;

        [Header("Sounds")]
        [SerializeField] private SOSoundSource forceOpenSound;
        [SerializeField] private SOSoundSource lockedFeedbackSound;

        [Header("Scene Transition")]
        [SerializeField] private string nextSceneName = "NextLevel";
        [SerializeField] private float animationWaitTime = 2f;

        #endregion

        #region Private Fields

        private bool isOpening = false;
        private readonly int hashForceOpen = Animator.StringToHash("ForceOpen");

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
            if (doorAnimator == null)
            {
                TryGetComponent(out doorAnimator);
            }

            // Find SceneLoader if not assigned
            if (sceneLoader == null)
            {
                sceneLoader = FindFirstObjectByType<SceneLoader>();
            }
        }

        #endregion


        #region Interactable Implementation

        public override void Interact()
        {
            if (isOpening)
            {
                Log.D("Door is already opening", _LOG_COLOR, _LOG_TAG_FULL);
                return;
            }

            if (puzzleController == null || !puzzleController.CanForceExitDoor())
            {
                // Show feedback: "You need all required items"
                PlayLockedFeedback();
                return;
            }

            // Force open the door
            ForceOpenDoor();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Forces open the exit door and transitions to the next scene
        /// </summary>
        private void ForceOpenDoor()
        {
            isOpening = true;

            // Play animation
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger(hashForceOpen);
            }
            else
            {
                Log.W($"DoorAnimator not found on {gameObject.name}", _LOG_COLOR, _LOG_TAG_FULL);
            }

            // Play sound
            if (forceOpenSound != null)
            {
                ECSound.PlaySoundAtPosition(forceOpenSound, transform.position, newAudioSphereEvent, "SFX");
            }

            // Wait for animation, then transition
            StartCoroutine(WaitForAnimationAndTransition());

            Log.D("Forcing open exit door", _LOG_COLOR, _LOG_TAG_FULL);
        }

        /// <summary>
        /// Waits for the door animation to complete, then transitions to the next scene
        /// </summary>
        private IEnumerator WaitForAnimationAndTransition()
        {
            // Wait for animation to complete (adjust time as needed)
            yield return new WaitForSeconds(animationWaitTime);

            // Load next scene
            if (sceneLoader != null)
            {
                StartCoroutine(sceneLoader.LoadLevelAdditive(nextSceneName));
            }
            else
            {
                // Fallback: direct scene loading
                Log.W($"SceneLoader not found. Loading scene directly: {nextSceneName}", _LOG_COLOR, _LOG_TAG_FULL);
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
        }

        /// <summary>
        /// Plays feedback sound when door is locked (puzzle not completed)
        /// </summary>
        private void PlayLockedFeedback()
        {
            if (lockedFeedbackSound != null)
            {
                ECSound.PlaySoundAtPosition(lockedFeedbackSound, transform.position, newAudioSphereEvent, "SFX");
            }
            Log.D("Door is locked - puzzle not completed", _LOG_COLOR, _LOG_TAG_FULL);
        }

        #endregion
    }
}

