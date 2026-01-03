using UnityEngine;

namespace EchoCity
{
    public class FirstLevelQuestManager : QuestsManager
    {
        protected override void Awake()
        {
            base.Awake();
            // Register level specific quest event handlers
            _onAddQuest[(int)QuestsEnum.TestOne] = OnTestOneAdded;
            _onCompleteQuest[(int)QuestsEnum.TestOne] = OnTestOneCompleted;

            _onAddQuest[(int)QuestsEnum.TestTwo] = OnTestTwoAdded;
            _onCompleteQuest[(int)QuestsEnum.TestTwo] = OnTestTwoCompleted;
        }

        //####################################################################
        private void OnTestOneAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += TestQuestHandler;
            }
        }

        private void TestQuestHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemAdded || pickable != PickablesEnum.CannedFood)
                return;

            IncrementQuestProgress(QuestsEnum.TestOne);
            if (questProgression[(int)QuestsEnum.TestOne] < activeQuests[(int)QuestsEnum.TestOne].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.PhonePicked, true) });
        }

        private void OnTestOneCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= TestQuestHandler;
            }
        }

        //####################################################################

        private void OnTestTwoAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += TestTwoQuestHandler;
            }
        }

        private void TestTwoQuestHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemDropped || pickable != PickablesEnum.CannedFood)
                return;
            Log.DLazy(() => $"TestTwoQuestHandler called from sender {sender.SenderName} with value {pickable}.", this);
            IncrementQuestProgress(QuestsEnum.TestTwo);
            if (questProgression[(int)QuestsEnum.TestTwo] < activeQuests[(int)QuestsEnum.TestTwo].CountToComplete)
                return;
            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.BunkerDoorKeyPicked, true) });
        }

        private void OnTestTwoCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= TestTwoQuestHandler;
            }
        }

        //####################################################################

    }
}