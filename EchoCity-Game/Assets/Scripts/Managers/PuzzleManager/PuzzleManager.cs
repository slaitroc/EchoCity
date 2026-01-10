using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public enum PuzzleTagEnum
    {
        NONE = 0,
        //TUTORIAL TAGS
        Move_TutorialCompleted = 1,
        Look_TutorialCompleted = 2,
        Sprint_TutorialCompleted = 3,
        Jump_TutorialCompleted = 4,
        Interact_TutorialCompleted = 5,
        PickUp_TutorialCompleted = 6,
        OpenInventory_TutorialCompleted = 7,
        EquipItem_TutorialCompleted = 8,
        UseItem_TutorialCompleted = 9,
        DropItem_TutorialCompleted = 10,
        SwitchLightsOff_TutorialCompleted = 11,
        UseLowSO_TutorialCompleted = 12,
        UseMidSO_TutorialCompleted = 13,
        UseHighSO_TutorialCompleted = 14,
        EnemySoundChase_TutorialCompleted = 15,
        TutorialCompleted = 29,
        //FIRST LEVEL TAGS
        // Picked
        Lighting_On = 30, // if false echo material must be used
        Phone_Picked = 31,
        WalkieTalkie_Picked = 32,
        BunkerDoorKey_Picked = 33,
        Cable_Picked = 34,
        FloppyDisk_Picked = 35,
        // On / Activated
        CardReader_On = 36,
        WearingGlasses = 37,
        MAX
    }

    [System.Serializable]
    public struct PuzzleTagState
    {
        [SerializeField] private PuzzleTagEnum _tag;
        [SerializeField] private bool _isActive;
        [SerializeField] private int _count;
        public PuzzleTagEnum Tag => _tag;
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public int Count { get => _count; set => _count = value; }

        public PuzzleTagState(PuzzleTagEnum tag, bool isActive, int count = 0)
        {
            _tag = tag;
            _isActive = isActive;
            _count = count;
        }
    }
    public class PuzzleManager : MonoBehaviour, IEventSender, IPuzzleManager
    {
        public string SenderName => gameObject.name;
        public int SenderID => GetInstanceID();
        public bool IsManager => true;
        public EventSenderCategoriesEnum[] SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Puzzle };


        [Header("Quests Management")]
        [SerializeField] private QuestsManager questsManager;
        public IQuestsManager QuestsManager => questsManager;
        [Header("Puzzle Tags")]
        [SerializeField] private PuzzleTagState[] puzzleTags;

        void Awake()
        {
            puzzleTags = new PuzzleTagState[(int)PuzzleTagEnum.MAX];
            Debug.Assert(puzzleTags != null && puzzleTags.Length > 0, "Active puzzle tags array is null or empty");
        }

        public bool TryUpdateTagsHandler(PuzzleTagState[] tagsToCheck, PuzzleTagState[] tagsToSet)
        {
            bool outcome = CheckTags(tagsToCheck);
            if (outcome)
                SetTags(tagsToSet);
            return outcome;
        }

        public void AddQuest(SOQuest quest)
        {
            questsManager?.AddQuest(quest);
        }

        public void AddToTagsTriggeredQuests(SOQuest quest)
        {
            questsManager?.AddToTagsTriggeredQuests(quest);
        }

        public void IncrementTagCount(PuzzleTagEnum tag)
        {
            if (tag == PuzzleTagEnum.NONE || tag == PuzzleTagEnum.MAX) return;
            puzzleTags[(int)tag].Count = puzzleTags[(int)tag].Count + 1;
            questsManager?.UpdateActiveQuests(this);
        }

        private void InitializeTags()
        {
            // Initialize all tags to inactive
            for (int i = 0; i < puzzleTags.Length; i++)
                puzzleTags[i] = new PuzzleTagState((PuzzleTagEnum)i, false);

            // Set specific tags to active at the start
            puzzleTags[(int)PuzzleTagEnum.Lighting_On].IsActive = true;
        }

        // if tagsToCheck is null or empty, return false
        // the NONE tag is used as a terminator, so if encountered, the check stops there
        // return true only if all tags in tagsToCheck match the current puzzleTags state
        public bool CheckTags(PuzzleTagState[] tagsToCheck)
        {
            if (tagsToCheck == null || tagsToCheck.Length == 0)
                return false;

            bool allTagsActive = true;
            for (int i = 0; i < tagsToCheck.Length; i++)
            {
                if (tagsToCheck[i].Tag == PuzzleTagEnum.NONE)
                    break;
                if (puzzleTags[(int)tagsToCheck[i].Tag].IsActive != tagsToCheck[i].IsActive ||
                    (tagsToCheck[i].Count > 0 && puzzleTags[(int)tagsToCheck[i].Tag].Count < tagsToCheck[i].Count))
                {
                    allTagsActive = false;
                    break;
                }
            }
            return allTagsActive;
        }

        public void SetTags(PuzzleTagState[] tagsToSet, bool checkQuests = true)
        {
            if (tagsToSet == null || tagsToSet.Length == 0) return;
            for (int i = 0; i < tagsToSet.Length; i++)
            {
                if (tagsToSet[i].Tag == PuzzleTagEnum.NONE) return;
                puzzleTags[(int)tagsToSet[i].Tag].IsActive = tagsToSet[i].IsActive;
                puzzleTags[(int)tagsToSet[i].Tag].Count = tagsToSet[i].Count;
            }
            if (checkQuests)
                questsManager?.UpdateActiveQuests(this);
        }

        public void ClearQuestsManager()
        {
            if (this.questsManager != null)
                this.questsManager.UnsubscribeAll();
            this.questsManager = null;
        }
        public void SetQuestsManager(QuestsManager questsManager)
        {
            ClearQuestsManager();
            InitializeTags();
            questsManager.SetPuzzleManager(this);
            this.questsManager = questsManager;
        }
    }

}