using System;
using UnityEngine;

namespace EchoCity
{
    public enum QuestsTypeEnum
    {
        Main,
        OneShot,
    }

    public enum QuestsEnum
    {
        //TUTORIAL QUESTS
        Move_Tutorial,
        Look_Tutorial,
        Sprint_Tutorial,
        Jump_Tutorial,
        Interact_Tutorial,
        PickUp_Tutorial,
        OpenInventory_Tutorial,
        EquipItem_Tutorial,
        UseItem_Tutorial,
        DropItem_Tutorial,
        SwitchLightsOff_Tutorial,
        UseLowSO_Tutorial,
        UseMidSO_Tutorial,
        UseHighSO_Tutorial,
        EnemySoundChase_Tutorial,
        //FIRST LEVEL QUESTS

        //OLD QUESTS
        TestOne,
        TestTwo,
        FindTheKey,
        OpenTheDoor,
        FixTheGenerator,
        TryEscape,
        FindPry,
        FindFloppy,
        FindPhone,
        FixGenerator,
        FindCable,
        Escape,
        MAX
    }

    [CreateAssetMenu(fileName = "Quest", menuName = "ECHO CITY/Puzzle/Quest")]
    public class SOQuest : ScriptableObject
    {
        [SerializeField] private QuestsTypeEnum questType = QuestsTypeEnum.Main;
        [SerializeField] private QuestsEnum quest = QuestsEnum.Escape;
        [SerializeField] private PuzzleTagState[] tagsToCheck;
        [SerializeField] private int countToComplete = 1;
        [SerializeField] private SOEventBase[] subscribeToEvents;
        [SerializeField] private SOQuest[] nextQuests;
        [TextArea, SerializeField] private string description;
        [TextArea, SerializeField] private string questCompletedText;
        [SerializeField] private SODialogContainer scriptContainer;

        public QuestsTypeEnum QuestType => questType;
        public QuestsEnum Quest => quest;
        public string Description => description;
        public string QuestCompletedText => questCompletedText;
        public PuzzleTagState[] TagsToCheck => tagsToCheck;
        public int CountToComplete => countToComplete;
        public SOEventBase[] SubscribeToEvents => subscribeToEvents;
        public SOQuest[] NextQuests => nextQuests;
        public SODialogContainer ScriptContainer => scriptContainer;


        void OnValidate()
        {
            if (tagsToCheck == null || tagsToCheck.Length == 0)
            {
                tagsToCheck = new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.NONE, true) };
            }

            string assetName = this.name;

            if (!string.IsNullOrEmpty(assetName))
            {
                // Remove "Quest" from the name (e.g: "FindTheKeyQuest" -> "FindTheKey")
                string cleanedName = assetName.Replace("Quest", "").Trim();

                // Try to convert the cleaned string to an Enum value
                if (Enum.TryParse(cleanedName, true, out QuestsEnum result))
                {
                    // If found, assign it
                    if (quest != result)
                    {
                        quest = result;
                        Debug.Log($"<color=green>Quest automatically assigned:</color> {result} for asset {assetName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"<color=black>No matching Enum value found for name: {cleanedName}");
                }
            }
        }
    }
}