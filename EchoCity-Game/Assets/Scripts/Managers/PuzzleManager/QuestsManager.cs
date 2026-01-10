using System;
using UnityEngine;

public enum QuestStateEnum
{
    Inactive = -1,
    Completed = -100,
    ResetQuestsManager = -999,
}

namespace EchoCity
{
    public abstract class QuestsManager : MonoBehaviour, IEventSender, IQuestsManager
    {
        [Header("Invoking Events")]
        [SerializeField] protected SOQuestUpdatedEvent questsUpdatedEvent;
        [SerializeField] protected SOShowUIEvent showUIEvent;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => true;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Puzzle };

        protected IPuzzleManager _puzzleManager;
        [SerializeField] protected SOQuest[] tagsTriggeredQuests;
        [SerializeField] protected SOQuest[] activeQuests;
        [SerializeField] protected int[] questProgression;
        protected Action<SOQuest>[] _onAddQuest;
        protected Action<SOQuest>[] _onCompleteQuest;
        protected Action<SOQuest>[] _onUnsubscribeQuest;

        [SerializeField] protected Color completeQuestMessageColor = Color.green;

        SOQuest[] IQuestsManager.ActiveQuests => activeQuests;
        int[] IQuestsManager.QuestProgression => questProgression;


        protected virtual void Awake()
        {
            if (tagsTriggeredQuests == null)
                tagsTriggeredQuests = new SOQuest[0];
            activeQuests = new SOQuest[(int)QuestsEnum.MAX];
            questProgression = new int[(int)QuestsEnum.MAX];
            for (int i = 0; i < questProgression.Length; i++)
                questProgression[i] = (int)QuestStateEnum.Inactive;
            _onAddQuest = new Action<SOQuest>[(int)QuestsEnum.MAX];
            _onCompleteQuest = new Action<SOQuest>[(int)QuestsEnum.MAX];
            _onUnsubscribeQuest = new Action<SOQuest>[(int)QuestsEnum.MAX];
        }

        public void SetPuzzleManager(IPuzzleManager puzzleManager)
        {
            this._puzzleManager = puzzleManager;
        }

        public void AddQuest(SOQuest quest, bool reAdd = false)
        {
            // Add the quest to the active quests array
            if (activeQuests[(int)quest.Quest] != null && !reAdd)
            {
                Log.DLazy(() => $"Quest {quest.name} is already active.", this);
                return;
            }
            activeQuests[(int)quest.Quest] = quest;
            // Initialize the quest progression counter
            questProgression[(int)quest.Quest] = 0;
            // Invoke any specific event handlers for the quest
            questsUpdatedEvent?.RaiseEvent(this, (int)quest.Quest, 0);
            _onAddQuest[(int)quest.Quest]?.Invoke(quest);
        }

        private void DisableQuest(SOQuest quest)
        {
            questProgression[(int)quest.Quest] = (int)QuestStateEnum.Inactive;
            _onUnsubscribeQuest[(int)quest.Quest]?.Invoke(quest);
            questsUpdatedEvent?.RaiseEvent(this, (int)quest.Quest, questProgression[(int)quest.Quest]);
        }

        public void CompleteQuest(SOQuest quest)
        {
            if (questProgression[(int)quest.Quest] == (int)QuestStateEnum.Completed)
                return;
            // Reset the quest progression counter
            questProgression[(int)quest.Quest] = (int)QuestStateEnum.Completed;
            // Invoke any specific event handlers for quest completion
            questsUpdatedEvent?.RaiseEvent(this, (int)quest.Quest, questProgression[(int)quest.Quest]);
            _onCompleteQuest[(int)quest.Quest]?.Invoke(quest);
            foreach (var nextQuest in quest.NextQuests)
                AddQuest(nextQuest);
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
            for (int i = 0; i < tagsTriggeredQuests.Length; i++)
            {
                var quest = tagsTriggeredQuests[i];
                if (quest == null)
                    continue;
                if (quest.TagsToActivate == null || quest.TagsToActivate.Length == 0)
                    continue;
                // Check if the quest conditions are met
                if (puzzleManager.CheckTags(quest.TagsToActivate))
                    AddQuest(quest, true);
                else
                    DisableQuest(quest);
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
            EchoCitySound.AddInVoicePlayQueue(quest.ScriptContainer, showUIEvent, lineIndex, @override);
        }

        protected void ShowCompletedMessage(SOQuest quest)
        {
            if (string.IsNullOrEmpty(quest.QuestCompletedText))
                return;
            showUIEvent.RaiseEvent(this, ShowableUIEnum.PopUpMessage, new PopUpMessageParams(quest.QuestCompletedText, completeQuestMessageColor));
        }

        public void UnsubscribeAll()
        {
            for (int i = 0; i < _onUnsubscribeQuest.Length; i++)
                _onUnsubscribeQuest[i]?.Invoke(activeQuests[i]);
            for (int i = 0; i < _onAddQuest.Length; i++)
                _onAddQuest[i] = null;
            for (int i = 0; i < _onCompleteQuest.Length; i++)
                _onCompleteQuest[i] = null;
            questsUpdatedEvent.RaiseEvent(this, 0, (int)QuestStateEnum.ResetQuestsManager);
        }
    }
}