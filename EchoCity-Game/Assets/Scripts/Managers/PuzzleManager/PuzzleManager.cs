using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public enum PuzzleTagEnum
    {
        //FIRST LEVEL TAGS
        None = 0,
        PhonePicked = 1,
        WalkieTalkiePicked = 2,
        BunkerDoorKeyPicked = 3,
        CablePicked = 4,
        FloppyDiskPicked = 5,
        CardReaderIsOn = 6
    }

    public struct PuzzleTagState
    {
        private PuzzleTagEnum _tag;
        private bool _isActive;

        public PuzzleTagEnum Tag => _tag;
        public bool IsActive { get => _isActive; set => _isActive = value; }

        public PuzzleTagState(PuzzleTagEnum tag, bool isActive)
        {
            this._tag = tag;
            this._isActive = isActive;
        }
    }
    public class PuzzleManager : MonoBehaviour
    {
#pragma warning disable CS0414
        private const string LOG_TAG = "PUZZLE MANAGER";
        private const string LOG_COLOR = "#33ff57ff";
#pragma warning restore CS0414

        [Header("Invoking Events")]
        [SerializeField] private SOBoolEvent interactionOutcomeEvent;
        [Header("Observing Events")]
        [SerializeField] private SOPuzzleTagEnumArrayEvent checkTagsEvent;
        [SerializeField] private SOPuzzleTagEnumArrayEvent setPuzzleTagsEvent;
        [Header("Puzzle Tags")]
        [SerializeField] private PuzzleTagState[] activePuzzleTags;

        void OnEnable()
        {
            if (checkTagsEvent != null) checkTagsEvent.OnEventRaised += CheckTagsHandler;
            if (setPuzzleTagsEvent != null) setPuzzleTagsEvent.OnEventRaised += SetTagsHandler;
        }

        private void CheckTagsHandler(PuzzleTagEnum[] tagsToCheck)
        {
            bool allTagsActive = true;
            foreach (PuzzleTagEnum tag in tagsToCheck)
            {
                foreach (PuzzleTagState activeTag in activePuzzleTags)
                {
                    if (tag == activeTag.Tag && !activeTag.IsActive)
                    {
                        allTagsActive = false;
                        break;
                    }
                }
                if (!allTagsActive) break;
            }
            interactionOutcomeEvent?.RaiseEvent(allTagsActive);
        }

        private void SetTagsHandler(PuzzleTagEnum[] tagsToSet)
        {
            if (activePuzzleTags == null || activePuzzleTags.Length == 0)
                Log.E(" No active puzzle tags defined.", LOG_TAG, LOG_COLOR);
            foreach (PuzzleTagEnum tag in tagsToSet)
            {
                for (int i = 0; i < activePuzzleTags.Length; i++)
                {
                    if (tag == activePuzzleTags[i].Tag)
                    {
                        activePuzzleTags[i].IsActive = true;
                        break;
                    }
                }
            }
        }


    }

}