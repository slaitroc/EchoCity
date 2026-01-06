using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStateEnum
{
    Inactive = -1,
    Completed = -100,
}

namespace EchoCity
{
    public abstract class QuestsManager : MonoBehaviour, IEventSender, IQuestsManager
    {
        [Header("Invoking Events")]
        [SerializeField] private SOQuestUpdatedEvent questsUpdatedEvent;
        [SerializeField] protected SOShowUIEvent showUIEvent;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => true;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Puzzle };

        [SerializeField] protected PuzzleManager puzzleManager;
        protected IPuzzleManager _puzzleManager => puzzleManager;
        [SerializeField] protected SOQuest[] activeQuests;
        [SerializeField] protected int[] questProgression;
        protected Action<SOQuest>[] _onAddQuest;
        protected Action<SOQuest>[] _onCompleteQuest;

        SOQuest[] IQuestsManager.ActiveQuests => activeQuests;
        int[] IQuestsManager.QuestProgression => questProgression;


        protected virtual void Awake()
        {
            activeQuests = new SOQuest[(int)QuestsEnum.MAX];
            questProgression = new int[(int)QuestsEnum.MAX];
            for (int i = 0; i < questProgression.Length; i++)
                questProgression[i] = (int)QuestStateEnum.Inactive;
            _onAddQuest = new Action<SOQuest>[(int)QuestsEnum.MAX];
            _onCompleteQuest = new Action<SOQuest>[(int)QuestsEnum.MAX];
        }

        public void AddQuest(SOQuest quest)
        {
            // Add the quest to the active quests array
            if (activeQuests[(int)quest.Quest] != null)
            {
                Log.DLazy(() => $"Quest {quest.name} is already active.", this);
                return;
            }
            activeQuests[(int)quest.Quest] = quest;
            // Initialize the quest progression counter
            questProgression[(int)quest.Quest] = 0;
            // Invoke any specific event handlers for the quest
            _onAddQuest[(int)quest.Quest]?.Invoke(quest);
            questsUpdatedEvent?.RaiseEvent(this, (int)quest.Quest, questProgression[(int)quest.Quest]);
        }

        public void CompleteQuest(SOQuest quest)
        {
            // Reset the quest progression counter
            questProgression[(int)quest.Quest] = (int)QuestStateEnum.Completed;
            // Invoke any specific event handlers for quest completion
            _onCompleteQuest[(int)quest.Quest]?.Invoke(quest);
            foreach (var nextQuest in quest.NextQuests)
                AddQuest(nextQuest);
            questsUpdatedEvent?.RaiseEvent(this, (int)quest.Quest, questProgression[(int)quest.Quest]);
        }

        public void UpdateActiveQuests(IPuzzleManager puzzleManager)
        {
            for (int i = 0; i < questProgression.Length; i++)
            {
                var code = questProgression[i];
                if (code != (int)QuestStateEnum.Inactive && code != (int)QuestStateEnum.Completed)
                {
                    var quest = activeQuests[i];
                    if (quest == null)
                        continue;
                    if (quest.TagsToCheck == null || quest.TagsToCheck.Length == 0)
                        continue;
                    // Check if the quest conditions are met
                    if (puzzleManager.CheckTags(quest.TagsToCheck))
                        CompleteQuest(quest);
                }
            }

        }

        protected void IncrementQuestProgress(QuestsEnum questEnum)
        {
            if (questProgression[(int)questEnum] >= 0)
            {
                questProgression[(int)questEnum]++;
                questsUpdatedEvent?.RaiseEvent(this, (int)questEnum, questProgression[(int)questEnum]);
            }
        }

        protected void PlayLine(SOQuest quest, int lineIndex, bool @override = false)
        {
            EchoCitySound.AddInVoicePlayQueue(quest, showUIEvent, lineIndex, @override);
        }
    }
}