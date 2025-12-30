using System;
using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(Collider))]
    public class TutorialTrigger : MonoBehaviour
    {
        [SerializeField] protected SODialogContainer tutorialDialogContainer;
        [SerializeField] protected SODialogDataEvent switchToNarrationStateEvent;
        [Header("Puzzle Tags")]
        [SerializeField] protected PuzzleManager puzzleManager;
        [SerializeField] protected PuzzleTagState[] checkTags;
        [SerializeField] protected PuzzleTagState[] setTags;


        void Start()
        {
            if (puzzleManager == null)
                puzzleManager = GameObject.FindWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
            Debug.Assert(puzzleManager != null, "PuzzleManager not found in the scene");
        }
        protected virtual void ResolveInteraction(bool outcome)
        {
            if (outcome)
            {
                switchToNarrationStateEvent?.RaiseEvent(new DialogData(tutorialDialogContainer));
                gameObject.SetActive(false);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            ResolveInteraction(puzzleManager?.TryUpdateTagsHandler(checkTags, setTags) ?? false);
        }

        void OnValidate()
        {
            if (puzzleManager == null)
                puzzleManager = GameObject.FindWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
        }
    }
}
