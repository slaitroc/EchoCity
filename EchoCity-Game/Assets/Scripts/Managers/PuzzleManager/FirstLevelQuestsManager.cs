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

            _onAddQuest[(int)QuestsEnum.Move_Tutorial] = OnMove_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.Move_Tutorial] = OnMove_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.Look_Tutorial] = OnLook_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.Look_Tutorial] = OnLook_TutorialCompleted;
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

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Phone_Picked, true) });
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
            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.BunkerDoorKey_Picked, true) });
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

        private void OnMove_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest Move_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised += MoveTutorialHandler;
            }
        }

        private void MoveTutorialHandler(IEventSender sender, MovementCodeEnum code)
        {
            if (code != MovementCodeEnum.Move)
                return;

            questProgression[(int)QuestsEnum.Move_Tutorial]++;
            if (questProgression[(int)QuestsEnum.Move_Tutorial] < activeQuests[(int)QuestsEnum.Move_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Move_TutorialCompleted, true) });
        }

        private void OnMove_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised -= MoveTutorialHandler;
            }
        }

        //####################################################################

        private void OnLook_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest Look_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised += LookTutorialHandler;
            }
        }

        private void LookTutorialHandler(IEventSender sender, MovementCodeEnum code)
        {
            if (code != MovementCodeEnum.Look)
                return;

            questProgression[(int)QuestsEnum.Look_Tutorial]++;
            if (questProgression[(int)QuestsEnum.Look_Tutorial] < activeQuests[(int)QuestsEnum.Look_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Look_TutorialCompleted, true) });
        }

        private void OnLook_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised -= LookTutorialHandler;
            }
        }
    }
}