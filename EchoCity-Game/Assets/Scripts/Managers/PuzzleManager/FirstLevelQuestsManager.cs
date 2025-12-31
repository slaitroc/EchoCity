using UnityEngine;

namespace EchoCity
{
    public class FirstLevelQuestManager : QuestsManager
    {
        protected override void Awake()
        {
            base.Awake();
            // Register level specific quest event handlers
            _onAddQuest[(int)QuestsEnum.TestOne] = OnAddTestQuest;
            _onCompleteQuest[(int)QuestsEnum.TestOne] = OnTestQuestCompleted;
        }

        private void OnAddTestQuest(SOQuest quest)
        {
            Log.DLazy(() => $"FirstLevelQuestManager: OnAddTestQuest called for quest {quest.name}.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.DropItem)
                    ((SOIntEvent)eventBase).OnEventRaised += TestQuestHandler;
            }
        }

        private void TestQuestHandler(IEventSender sender, int value)
        {
            Log.DLazy(() => $"FirstLevelQuestManager: TestQuestHandler called from sender {sender.SenderName} with value {value}.", this);
            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.PhonePicked, true) });
            puzzleManager.IncrementTagCount(PuzzleTagEnum.PhonePicked);
        }

        private void OnTestQuestCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"FirstLevelQuestManager: OnTestQuestCompleted called for quest {quest.name}.", this);
        }

    }
}