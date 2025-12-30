using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public enum PuzzleTagEnum
    {
        //FIRST LEVEL TAGS
        NONE,
        PhonePicked,
        WalkieTalkiePicked,
        BunkerDoorKeyPicked,
        CablePicked,
        FloppyDiskPicked,
        CardReaderIsOn,
        LightingEnabled,
        MAX
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
            _tag = tag;
            _isActive = isActive;
        }
    }
    public class PuzzleManager : MonoBehaviour, IEventSender
    {
        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => true;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Puzzle };

        [Header("Puzzle Tags")]
        [SerializeField] private PuzzleTagState[] puzzleTags;


        void Awake()
        {
            puzzleTags = new PuzzleTagState[(int)PuzzleTagEnum.MAX];
            InitializeTags();
            Debug.Assert(puzzleTags != null && puzzleTags.Length > 0, "Active puzzle tags array is null or empty");
        }

        public bool TryUpdateTagsHandler(PuzzleTagState[] tagsToCheck, PuzzleTagState[] tagsToSet)
        {
            bool outcome = CheckTags(tagsToCheck);
            if (outcome)
                SetTags(tagsToSet);
            return outcome;
        }

        private void InitializeTags()
        {
            // Initialize all tags to inactive
            for (int i = 0; i < puzzleTags.Length; i++)
                puzzleTags[i] = new PuzzleTagState((PuzzleTagEnum)i, false);

            // Set specific tags to active at the start
            puzzleTags[(int)PuzzleTagEnum.LightingEnabled].IsActive = true;
        }

        // if tagsToCheck is null or empty, return false
        // the NONE tag is used as a terminator, so if encountered, the check stops there
        // return true only if all tags in tagsToCheck match the current puzzleTags state
        private bool CheckTags(PuzzleTagState[] tagsToCheck)
        {
            if (tagsToCheck == null || tagsToCheck.Length == 0)
                return false;

            bool allTagsActive = true;
            for (int i = 0; i < tagsToCheck.Length; i++)
            {
                if (tagsToCheck[i].Tag == PuzzleTagEnum.NONE)
                    break;
                if (puzzleTags[(int)tagsToCheck[i].Tag].IsActive != tagsToCheck[i].IsActive)
                {
                    allTagsActive = false;
                    break;
                }
            }
            return allTagsActive;
        }

        private void SetTags(PuzzleTagState[] tagsToSet)
        {
            if (tagsToSet == null || tagsToSet.Length == 0) return;
            for (int i = 0; i < tagsToSet.Length; i++)
            {
                if (tagsToSet[i].Tag == PuzzleTagEnum.NONE) return;
                puzzleTags[(int)tagsToSet[i].Tag].IsActive = tagsToSet[i].IsActive;
            }
        }


    }

}