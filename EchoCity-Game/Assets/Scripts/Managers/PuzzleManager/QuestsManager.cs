using System;
using UnityEngine;

namespace EchoCity
{
    public abstract class QuestsManager : MonoBehaviour
    {
        [SerializeField] protected PuzzleManager puzzleManager;
        protected IPuzzleManager _puzzleManager => puzzleManager;
        [SerializeField] protected SOQuest[] activeQuests;
        protected Action<SOQuest>[] _onAddQuest;
        protected Action<SOQuest>[] _onCompleteQuest;

        protected virtual void Awake()
        {
            activeQuests = new SOQuest[(int)QuestsEnum.MAX];
            _onAddQuest = new Action<SOQuest>[(int)QuestsEnum.MAX];
            _onCompleteQuest = new Action<SOQuest>[(int)QuestsEnum.MAX];
        }

        public void AddQuest(SOQuest quest)
        {
            // Add the quest to the active quests array
            activeQuests[(int)quest.Quest] = quest;
            // Invoke any specific event handlers for the quest
            _onAddQuest[(int)quest.Quest]?.Invoke(quest);
        }

        public void CompleteQuest(SOQuest quest)
        {
            // Remove the quest from the active quests array
            activeQuests[(int)quest.Quest] = null;
            // Invoke any specific event handlers for quest completion
            _onCompleteQuest[(int)quest.Quest]?.Invoke(quest);
            foreach (var nextQuest in quest.NextQuests)
            {
                AddQuest(nextQuest);
            }
        }

        public void UpdateActiveQuests(IPuzzleManager puzzleManager)
        {
            for (int i = 0; i < activeQuests.Length; i++)
            {
                var quest = activeQuests[i];
                if (quest != null)
                {
                    if (puzzleManager.CheckTags(quest.TagsToCheck))
                        CompleteQuest(quest);
                }
            }

        }
    }
}