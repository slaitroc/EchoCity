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
        CardReaderIsOn = 6,
        LightsOff = 7
    }

    [System.Serializable]
    public struct PuzzleTagState
    {
        [SerializeField] private PuzzleTagEnum _tag;
        [SerializeField] private bool _isActive;
        public PuzzleTagEnum Tag => _tag;
        public bool IsActive { get => _isActive; set => _isActive = value; }

        public PuzzleTagState(PuzzleTagEnum tag, bool isActive)
        {
            this._tag = tag;
            this._isActive = isActive;

        }
    }
    public class PuzzleManager : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] private SOBoolEvent interactionOutcomeEvent;

        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => true;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Puzzle };

        [Header("Observing Events")]
        [SerializeField] private SOPuzzleTagEnumArrayEvent checkTagsEvent;
        [SerializeField] private SOPuzzleTagEnumArrayEvent setPuzzleTagsEvent;
        [Header("Puzzle Tags")]
        [SerializeField]
        private PuzzleTagState[] activePuzzleTags;


        void Awake()
        {
            activePuzzleTags = new PuzzleTagState[]{
                new PuzzleTagState(PuzzleTagEnum.PhonePicked, false),
                new PuzzleTagState(PuzzleTagEnum.WalkieTalkiePicked, false),
                new PuzzleTagState(PuzzleTagEnum.BunkerDoorKeyPicked, false),
                new PuzzleTagState(PuzzleTagEnum.CablePicked, false),
                new PuzzleTagState(PuzzleTagEnum.FloppyDiskPicked, false),
                new PuzzleTagState(PuzzleTagEnum.CardReaderIsOn, false),
                new PuzzleTagState(PuzzleTagEnum.LightsOff, false)
            };
        }
        void OnEnable()
        {
            if (checkTagsEvent != null) checkTagsEvent.OnEventRaised += CheckTagsHandler;
            if (setPuzzleTagsEvent != null) setPuzzleTagsEvent.OnEventRaised += SetTagsHandler;
        }

        private void CheckTagsHandler(IEventSender sender, PuzzleTagEnum[] tagsToCheck)
        {
            bool allTagsActive = true;
            if (tagsToCheck == null || tagsToCheck.Length == 0)
            {
                interactionOutcomeEvent?.RaiseEvent(this, false);
                return;
            }
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
            interactionOutcomeEvent?.RaiseEvent(this, allTagsActive);
        }

        private void SetTagsHandler(IEventSender sender, PuzzleTagEnum[] tagsToSet)
        {
            Debug.Assert(activePuzzleTags != null && activePuzzleTags.Length > 0, "Active puzzle tags array is null or empty");
            if (tagsToSet == null || tagsToSet.Length == 0) return;
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