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

        //####################################################################
        private void OnTryEscapeAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised += TryEscapeQuestHandler;
            }
        }

        private void OnTryEscapeCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
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

        //####################################################################

        private void OnFindPryAdded(SOQuest quest) => Log.DLazy(() => $"Quest {quest.name} added.", this);
        private void OnFindPryCompleted(SOQuest quest) => Log.DLazy(() => $"Quest {quest.name} completed.", this);

        //####################################################################

        private void OnFindFloppyAdded(SOQuest quest) => Log.DLazy(() => $"Quest {quest.name} added.", this);
        private void OnFindFloppyCompleted(SOQuest quest) => Log.DLazy(() => $"Quest {quest.name} completed.", this);

        //####################################################################

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

        //####################################################################

        private void OnFixGeneratorAdded(SOQuest quest) => Log.DLazy(() => $"Quest {quest.name} added.", this);
        private void OnFixGeneratorCompleted(SOQuest quest) => Log.DLazy(() => $"Quest {quest.name} completed.", this);

        //####################################################################

        private void OnEscapeAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised += EscapeQuestHandler;
            }
        }

        private void OnEscapeCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
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
    }
}