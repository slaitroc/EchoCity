using UnityEngine;

namespace EchoCity
{
    public class FirstLevelQuestManager : QuestsManager
    {
        protected override void Awake()
        {
            base.Awake();
            // Register level specific quest event handlers
            _onAddQuest[(int)QuestsEnum.FindPry] = OnFindPryAdded;
            _onCompleteQuest[(int)QuestsEnum.FindPry] = OnFindPryCompleted;

            _onAddQuest[(int)QuestsEnum.FindFloppy] = OnFindFloppyAdded;
            _onCompleteQuest[(int)QuestsEnum.FindFloppy] = OnFindFloppyCompleted;

            _onAddQuest[(int)QuestsEnum.FindPhone] = OnFindPhoneAdded;
            _onCompleteQuest[(int)QuestsEnum.FindPhone] = OnFindPhoneCompleted;

            _onAddQuest[(int)QuestsEnum.FixGenerator] = OnUseGeneratorAdded;
            _onCompleteQuest[(int)QuestsEnum.FixGenerator] = OnUseGeneratorCompleted;

            _onAddQuest[(int)QuestsEnum.EscapeFirstArea] = OnEscapeFirstAreaAdded;
            _onCompleteQuest[(int)QuestsEnum.EscapeFirstArea] = OnEscapeFirstAreaCompleted;

            _onAddQuest[(int)QuestsEnum.FindCable] = OnFindCableAdded;
            _onCompleteQuest[(int)QuestsEnum.FindCable] = OnFindCableCompleted;
        }

        #region Find Pry Quest
        private void OnFindPryAdded(SOQuest quest)
        {
            _onUnsubscribeQuest[(int)QuestsEnum.FindPry] = UnsubscribeFindPryHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeFindPryHandler(SOQuest quest) { }

        private void OnFindPryCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindPryHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region Find Floppy Quest
        private void OnFindFloppyAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.FindFloppy] = UnsubscribeFindFloppyHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeFindFloppyHandler(SOQuest quest) { }

        private void OnFindFloppyCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindFloppyHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region Find Phone Quest
        private void OnFindPhoneAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest FindPhone added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.FindPhone] = UnsubscribeFindPhoneHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeFindPhoneHandler(SOQuest quest) { }

        private void OnFindPhoneCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindPhoneHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region Use Generator Quest
        private void OnUseGeneratorAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.FixGenerator] = UnsubscribeUseGeneratorHandler;
            PlayLine(quest, 0);
        }
        private void UnsubscribeUseGeneratorHandler(SOQuest quest) { }

        private void OnUseGeneratorCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeUseGeneratorHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region Escape First Area Quest
        private void OnEscapeFirstAreaAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised += EscapeFirstAreaHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.EscapeFirstArea] = UnsubscribeEscapeFirstAreaHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeEscapeFirstAreaHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised -= EscapeFirstAreaHandler;
            }
        }

        private void EscapeFirstAreaHandler(IEventSender sender, InteractionEnum interaction)
        {
            if (interaction != InteractionEnum.LaboratoryAreaDoor)
                return;
            if (questProgression[(int)QuestsEnum.EscapeFirstArea] >= activeQuests[(int)QuestsEnum.EscapeFirstArea].CountToComplete)
                return;
            if (_puzzleManager.CheckTags(activeQuests[(int)QuestsEnum.EscapeFirstArea].TagsToActivate) == true)
            {
                _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.ExitFirstArea, true) }, true);
                CompleteQuest(activeQuests[(int)QuestsEnum.EscapeFirstArea]);
            }
        }

        private void OnEscapeFirstAreaCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeEscapeFirstAreaHandler(quest);
            _puzzleManager.RemoveFromTagsTriggeredQuests(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }

        #endregion

        #region Find Cable Quest
        private void OnFindCableAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.FindCable] = UnsubscribeFindCableHandler;
            PlayLine(quest, 0);
        }
        private void UnsubscribeFindCableHandler(SOQuest quest) { }

        private void OnFindCableCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindCableHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion
    }
}