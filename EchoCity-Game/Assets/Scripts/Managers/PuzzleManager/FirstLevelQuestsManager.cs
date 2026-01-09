using UnityEngine;

namespace EchoCity
{
    public class FirstLevelQuestManager : QuestsManager
    {
        [SerializeField] private SOQuest startQuest;
        [SerializeField] private SOQuest finalQuest;
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

        private void Start()
        {
            if (startQuest != null) AddQuest(startQuest);
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

        private void TryEscapeQuestHandler(IEventSender sender, InteractionsEnum interaction)
        {
            if (interaction == InteractionsEnum.EndDoor)
                CompleteQuest(activeQuests[(int)QuestsEnum.TryEscape]);
        }

        //####################################################################

        private void OnFindPryAdded(SOQuest quest) => Log.DLazy(() => $"Quest {quest.name} added.", this);
        private void OnFindPryCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            TryStartFinalQuest();
        }

        //####################################################################

        private void OnFindFloppyAdded(SOQuest quest) => Log.DLazy(() => $"Quest {quest.name} added.", this);
        private void OnFindFloppyCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            TryStartFinalQuest();
        }

        //####################################################################

        private void OnFindPhoneAdded(SOQuest quest) => Log.DLazy(() => $"Quest {quest.name} added.", this);
        private void OnFindPhoneCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            TryStartFinalQuest();
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

        private void EscapeQuestHandler(IEventSender sender, InteractionsEnum interaction)
        {
            if (interaction == InteractionsEnum.EndDoor)
                CompleteQuest(activeQuests[(int)QuestsEnum.Escape]);
        }

        //####################################################################

        private void TryStartFinalQuest()
        {
            bool pryCompleted = questProgression[(int)QuestsEnum.FindPry] == (int)QuestStateEnum.Completed;
            bool floppyCompleted = questProgression[(int)QuestsEnum.FindFloppy] == (int)QuestStateEnum.Completed;
            bool phoneCompleted = questProgression[(int)QuestsEnum.FindPhone] == (int)QuestStateEnum.Completed;

            if (!pryCompleted || !floppyCompleted || !phoneCompleted)
                return;

            if (questProgression[(int)finalQuest.Quest] != (int)QuestStateEnum.Inactive)
                return;

            AddQuest(finalQuest);
        }
    }
}