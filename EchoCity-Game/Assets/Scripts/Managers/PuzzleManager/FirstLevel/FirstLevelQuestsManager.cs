using UnityEngine;

namespace EchoCity
{
    public class FirstLevelQuestManager : QuestsManager
    {
        protected override void Awake()
        {
            base.Awake();
            // Register level specific quest event handlers
            _onAddQuest[(int)QuestsEnum.TryEscape] = OnTryEscapeAdded;
            _onCompleteQuest[(int)QuestsEnum.TryEscape] = OnTryEscapeCompleted;

            _onAddQuest[(int)QuestsEnum.FindPry] = OnFindPryAdded;
            _onCompleteQuest[(int)QuestsEnum.FindPry] = OnFindPryCompleted;

            _onAddQuest[(int)QuestsEnum.FindFloppy] = OnFindFloppyAdded;
            _onCompleteQuest[(int)QuestsEnum.FindFloppy] = OnFindFloppyCompleted;

            _onAddQuest[(int)QuestsEnum.FindPhone] = OnFindPhoneAdded;
            _onCompleteQuest[(int)QuestsEnum.FindPhone] = OnFindPhoneCompleted;

            _onAddQuest[(int)QuestsEnum.FixGenerator] = OnFixGeneratorAdded;
            _onCompleteQuest[(int)QuestsEnum.FixGenerator] = OnFixGeneratorCompleted;

            _onAddQuest[(int)QuestsEnum.Escape] = OnEscapeAdded;
            _onCompleteQuest[(int)QuestsEnum.Escape] = OnEscapeCompleted;
        }

        #region Try Escape Quest
        private void OnTryEscapeAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised += TryEscapeQuestHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.TryEscape] = UnsubscribeTryEscapeHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeTryEscapeHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised -= TryEscapeQuestHandler;
            }
        }

        private void TryEscapeQuestHandler(IEventSender sender, InteractionEnum interaction)
        {
            Log.DLazy(() => $"TryEscapeQuestHandler received interaction: {interaction}", this);
            if (interaction == InteractionEnum.EndDoor)
                CompleteQuest(activeQuests[(int)QuestsEnum.TryEscape]);
        }

        private void OnTryEscapeCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeTryEscapeHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }

        #endregion

        #region Find Pry Quest
        private void OnFindPryAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                {
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= DropPryHandler;
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += FindPryHandler;
                }
            }
            _onUnsubscribeQuest[(int)QuestsEnum.FindPry] = UnsubscribeFindPryHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeFindPryHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                {
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= FindPryHandler;
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += DropPryHandler;
                }
            }
        }

        private void FindPryHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemAdded || pickable != PickablesEnum.CrowBar)
                return;

            IncrementQuestProgress(QuestsEnum.FindPry);
            if (questProgression[(int)QuestsEnum.FindPry] < activeQuests[(int)QuestsEnum.FindPry].CountToComplete)
                return;
            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.PryTool_Picked, true) });
            CompleteQuest(activeQuests[(int)QuestsEnum.FindPry]);
        }

        private void DropPryHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemDropped || pickable != PickablesEnum.CrowBar)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.PryTool_Picked, false) }, true);
        }

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
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                {
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= DropFloppyHandler;
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += FindFloppyHandler;
                }
            }
            _onUnsubscribeQuest[(int)QuestsEnum.FindFloppy] = UnsubscribeFindFloppyHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeFindFloppyHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                {
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= FindFloppyHandler;
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += DropFloppyHandler;
                }
            }
        }

        private void FindFloppyHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemAdded || pickable != PickablesEnum.FloppyDisk)
                return;

            IncrementQuestProgress(QuestsEnum.FindFloppy);
            if (questProgression[(int)QuestsEnum.FindFloppy] < activeQuests[(int)QuestsEnum.FindFloppy].CountToComplete)
                return;
            CompleteQuest(activeQuests[(int)QuestsEnum.FindFloppy]);
        }

        private void DropFloppyHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemDropped || pickable != PickablesEnum.FloppyDisk)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.FloppyDisk_Picked, false) }, true);
        }

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
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                {
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= DropPhoneHandler;
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += FindPhoneHandler;
                }
            }
            Log.DLazy(() => $"Quest FindPhone added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.FindPhone] = UnsubscribeFindPhoneHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeFindPhoneHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                {
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= FindPhoneHandler;
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += DropPhoneHandler;
                }
            }
        }

        private void FindPhoneHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemAdded || pickable != PickablesEnum.WalkieTalkie)
                return;

            IncrementQuestProgress(QuestsEnum.FindPhone);
            if (questProgression[(int)QuestsEnum.FindPhone] < activeQuests[(int)QuestsEnum.FindPhone].CountToComplete)
                return;
            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.WalkieTalkie_Picked, true) });
            CompleteQuest(activeQuests[(int)QuestsEnum.FindPhone]);
        }

        private void DropPhoneHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemDropped || pickable != PickablesEnum.WalkieTalkie)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.WalkieTalkie_Picked, false) }, true);
        }

        private void OnFindPhoneCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindPhoneHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region Fix Generator Quest
        private void OnFixGeneratorAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.FixGenerator] = UnsubscribeFixGeneratorHandler;
            PlayLine(quest, 0);
        }
        private void UnsubscribeFixGeneratorHandler(SOQuest quest) { }

        private void OnFixGeneratorCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFixGeneratorHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region Escape Quest
        private void OnEscapeAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised += EscapeQuestHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.Escape] = UnsubscribeEscapeHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeEscapeHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised -= EscapeQuestHandler;
            }
        }

        private void EscapeQuestHandler(IEventSender sender, InteractionEnum interaction)
        {
            if (interaction == InteractionEnum.EndDoor)
                CompleteQuest(activeQuests[(int)QuestsEnum.Escape]);
        }

        private void OnEscapeCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeEscapeHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }

        #endregion
    }
}