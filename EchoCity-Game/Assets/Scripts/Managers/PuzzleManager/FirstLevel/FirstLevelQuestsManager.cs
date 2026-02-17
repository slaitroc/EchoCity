using System.Collections;
using UnityEngine;

namespace EchoCity
{
    public class FirstLevelQuestManager : QuestsManager
    {
        [SerializeField] private SceneEnum destinationScene;
        [SerializeField] private SOSwitchLevelEvent switchLevelEvent;
        private bool[] _addAudioAlreadyPlayed;
        private bool[] _completeAudioAlreadyPlayed;
        protected override void Awake()
        {
            base.Awake();
            if (_addAudioAlreadyPlayed == null)
                _addAudioAlreadyPlayed = new bool[(int)QuestsEnum.MAX];
            if (_completeAudioAlreadyPlayed == null)
                _completeAudioAlreadyPlayed = new bool[(int)QuestsEnum.MAX];

            for (int i = 0; i < (int)QuestsEnum.MAX; i++)
            {
                _addAudioAlreadyPlayed[i] = false;
                _completeAudioAlreadyPlayed[i] = false;
            }

            // Register level specific quest event handlers
            _onAddQuest[(int)QuestsEnum.FindPry] = OnFindPryAdded;
            _onCompleteQuest[(int)QuestsEnum.FindPry] = OnFindPryCompleted;

            _onAddQuest[(int)QuestsEnum.FindFloppy] = OnFindFloppyAdded;
            _onCompleteQuest[(int)QuestsEnum.FindFloppy] = OnFindFloppyCompleted;

            _onAddQuest[(int)QuestsEnum.FindPhone] = OnFindPhoneAdded;
            _onCompleteQuest[(int)QuestsEnum.FindPhone] = OnFindPhoneCompleted;

            _onAddQuest[(int)QuestsEnum.FindCable] = OnFindCableAdded;
            _onCompleteQuest[(int)QuestsEnum.FindCable] = OnFindCableCompleted;

            _onAddQuest[(int)QuestsEnum.FindID] = OnFindIDAdded;
            _onCompleteQuest[(int)QuestsEnum.FindID] = OnFindIDCompleted;

            _onAddQuest[(int)QuestsEnum.FindDataCenterLaptop] = OnFindDataCenterLaptopAdded;
            _onCompleteQuest[(int)QuestsEnum.FindDataCenterLaptop] = OnFindDataCenterLaptopCompleted;




            _onAddQuest[(int)QuestsEnum.UseGenerator] = OnUseGeneratorAdded;
            _onCompleteQuest[(int)QuestsEnum.UseGenerator] = OnUseGeneratorCompleted;

            _onAddQuest[(int)QuestsEnum.EscapeFirstArea] = OnEscapeFirstAreaAdded;
            _onCompleteQuest[(int)QuestsEnum.EscapeFirstArea] = OnEscapeFirstAreaCompleted;

            _onAddQuest[(int)QuestsEnum.EnableEscapeDoorGenerator] = OnEnableEscapeDoorGeneratorAdded;
            _onCompleteQuest[(int)QuestsEnum.EnableEscapeDoorGenerator] = OnEnableEscapeDoorGeneratorCompleted;

            _onAddQuest[(int)QuestsEnum.Escape] = OnEscapeAdded;
            _onCompleteQuest[(int)QuestsEnum.Escape] = OnEscapeCompleted;



        }

        #region Find Pry Quest
        private void OnFindPryAdded(SOQuest quest)
        {
            UnsubscribeFindPryHandler(quest);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += FindPryHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.FindPry] = UnsubscribeFindPryHandler;
            if (!_addAudioAlreadyPlayed[(int)QuestsEnum.FindPry])
            {
                PlayLine(quest, 0);
                _addAudioAlreadyPlayed[(int)QuestsEnum.FindPry] = true;
            }
        }

        private void UnsubscribeFindPryHandler(SOQuest quest) { }

        private void FindPryHandler(IEventSender sender, PickablesEnum item, PickableTypeEnum itemType, InventoryCodesEnum code)
        {
            if (code != InventoryCodesEnum.ItemAdded || item != PickablesEnum.CrowBar)
                return;
            CompleteQuest(activeQuests[(int)QuestsEnum.FindPry]);
        }

        private void OnFindPryCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindPryHandler(quest);
            ShowCompletedMessage(quest);
            if (!_completeAudioAlreadyPlayed[(int)QuestsEnum.FindPry])
            {
                PlayLine(quest, 1, true);
                _completeAudioAlreadyPlayed[(int)QuestsEnum.FindPry] = true;
            }
        }
        #endregion

        #region Find Floppy Quest
        private void OnFindFloppyAdded(SOQuest quest)
        {
            UnsubscribeFindFloppyHandler(quest);
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += FindFloppyHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.FindFloppy] = UnsubscribeFindFloppyHandler;
            if (!_addAudioAlreadyPlayed[(int)QuestsEnum.FindFloppy])
            {
                PlayLine(quest, 0);
                _addAudioAlreadyPlayed[(int)QuestsEnum.FindFloppy] = true;
            }
        }

        private void FindFloppyHandler(IEventSender sender, PickablesEnum item, PickableTypeEnum itemType, InventoryCodesEnum code)
        {
            if (code != InventoryCodesEnum.ItemAdded || item != PickablesEnum.FloppyDisk)
                return;
            CompleteQuest(activeQuests[(int)QuestsEnum.FindFloppy]);
        }

        private void UnsubscribeFindFloppyHandler(SOQuest quest) { }

        private void OnFindFloppyCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindFloppyHandler(quest);
            ShowCompletedMessage(quest);
            if (!_completeAudioAlreadyPlayed[(int)QuestsEnum.FindFloppy])
            {
                PlayLine(quest, 1, true);
                _completeAudioAlreadyPlayed[(int)QuestsEnum.FindFloppy] = true;
            }
        }
        #endregion

        #region Find Phone Quest
        private void OnFindPhoneAdded(SOQuest quest)
        {
            UnsubscribeFindPhoneHandler(quest);
            Log.DLazy(() => $"Quest FindPhone added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.FindPhone] = UnsubscribeFindPhoneHandler;
            if (!_addAudioAlreadyPlayed[(int)QuestsEnum.FindPhone])
            {
                PlayLine(quest, 0);
                _addAudioAlreadyPlayed[(int)QuestsEnum.FindPhone] = true;
            }
        }

        private void UnsubscribeFindPhoneHandler(SOQuest quest) { }

        private void OnFindPhoneCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindPhoneHandler(quest);
            ShowCompletedMessage(quest);
            if (!_completeAudioAlreadyPlayed[(int)QuestsEnum.FindPhone])
            {
                PlayLine(quest, 1, true);
                _completeAudioAlreadyPlayed[(int)QuestsEnum.FindPhone] = true;
            }
        }
        #endregion

        #region Find Cable Quest
        private void OnFindCableAdded(SOQuest quest)
        {
            UnsubscribeFindCableHandler(quest);
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.FindCable] = UnsubscribeFindCableHandler;
            if (!_addAudioAlreadyPlayed[(int)QuestsEnum.FindCable])
            {
                PlayLine(quest, 0);
                _addAudioAlreadyPlayed[(int)QuestsEnum.FindCable] = true;
            }
        }
        private void UnsubscribeFindCableHandler(SOQuest quest) { }

        private void OnFindCableCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindCableHandler(quest);
            ShowCompletedMessage(quest);
            if (!_completeAudioAlreadyPlayed[(int)QuestsEnum.FindCable])
            {
                PlayLine(quest, 1, true);
                _completeAudioAlreadyPlayed[(int)QuestsEnum.FindCable] = true;
            }
        }
        #endregion

        #region Find ID Quest
        private void OnFindIDAdded(SOQuest quest)
        {
            UnsubscribeFindIDHandler(quest);
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.FindID] = UnsubscribeFindIDHandler;
            if (!_addAudioAlreadyPlayed[(int)QuestsEnum.FindID])
            {
                PlayLine(quest, 0);
                _addAudioAlreadyPlayed[(int)QuestsEnum.FindID] = true;
            }
        }
        private void UnsubscribeFindIDHandler(SOQuest quest) { }

        private void OnFindIDCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindIDHandler(quest);
            ShowCompletedMessage(quest);
            if (!_completeAudioAlreadyPlayed[(int)QuestsEnum.FindID])
            {
                PlayLine(quest, 1, true);
                _completeAudioAlreadyPlayed[(int)QuestsEnum.FindID] = true;
            }
        }
        #endregion

        #region Find DataCenter Laptop Quest
        private void OnFindDataCenterLaptopAdded(SOQuest quest)
        {
            UnsubscribeFindDataCenterLaptopHandler(quest);
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.FindDataCenterLaptop] = UnsubscribeFindDataCenterLaptopHandler;
            if (!_addAudioAlreadyPlayed[(int)QuestsEnum.FindDataCenterLaptop])
            {
                PlayLine(quest, 0);
                _addAudioAlreadyPlayed[(int)QuestsEnum.FindDataCenterLaptop] = true;
            }
        }
        private void UnsubscribeFindDataCenterLaptopHandler(SOQuest quest) { }

        private void OnFindDataCenterLaptopCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindDataCenterLaptopHandler(quest);
            ShowCompletedMessage(quest);
            if (!_completeAudioAlreadyPlayed[(int)QuestsEnum.FindDataCenterLaptop])
            {
                PlayLine(quest, 1, true);
                _completeAudioAlreadyPlayed[(int)QuestsEnum.FindDataCenterLaptop] = true;
            }
        }
        #endregion

        #region Use Generator Quest
        private void OnUseGeneratorAdded(SOQuest quest)
        {
            UnsubscribeUseGeneratorHandler(quest);
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.UseGenerator] = UnsubscribeUseGeneratorHandler;
            if (!_addAudioAlreadyPlayed[(int)QuestsEnum.UseGenerator])
            {
                PlayLine(quest, 0);
                _addAudioAlreadyPlayed[(int)QuestsEnum.UseGenerator] = true;
            }
        }

        private void UnsubscribeUseGeneratorHandler(SOQuest quest) { }

        private void OnUseGeneratorCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeUseGeneratorHandler(quest);
            ShowCompletedMessage(quest);
            if (!_completeAudioAlreadyPlayed[(int)QuestsEnum.UseGenerator])
            {
                PlayLine(quest, 1, true);
                _completeAudioAlreadyPlayed[(int)QuestsEnum.UseGenerator] = true;
            }
        }
        #endregion

        #region Escape First Area Quest
        private void OnEscapeFirstAreaAdded(SOQuest quest)
        {
            UnsubscribeEscapeFirstAreaHandler(quest);
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.EscapeFirstArea] = UnsubscribeEscapeFirstAreaHandler;
            if (!_addAudioAlreadyPlayed[(int)QuestsEnum.EscapeFirstArea])
            {
                PlayLine(quest, 0);
                _addAudioAlreadyPlayed[(int)QuestsEnum.EscapeFirstArea] = true;
            }
        }

        private void UnsubscribeEscapeFirstAreaHandler(SOQuest quest) { }

        private void OnEscapeFirstAreaCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeEscapeFirstAreaHandler(quest);
            _puzzleManager.RemoveFromTagsTriggeredQuests(quest);
            ShowCompletedMessage(quest);
            if (!_completeAudioAlreadyPlayed[(int)QuestsEnum.EscapeFirstArea])
            {
                PlayLine(quest, 1, true);
                _completeAudioAlreadyPlayed[(int)QuestsEnum.EscapeFirstArea] = true;
            }
        }

        #endregion

        #region Enable Escape Door Generator Quest
        private void OnEnableEscapeDoorGeneratorAdded(SOQuest quest)
        {
            UnsubscribeEnableEscapeDoorGeneratorHandler(quest);
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.EnableEscapeDoorGenerator] = UnsubscribeEnableEscapeDoorGeneratorHandler;
            if (!_addAudioAlreadyPlayed[(int)QuestsEnum.EnableEscapeDoorGenerator])
            {
                PlayLine(quest, 0);
                _addAudioAlreadyPlayed[(int)QuestsEnum.EnableEscapeDoorGenerator] = true;
            }
        }

        private IEnumerator TimeToReactivateGeneratorCoroutine(SOQuest quest)
        {
            yield return new WaitForSeconds(10f);
            _puzzleManager.SetTags(activeQuests[(int)QuestsEnum.EnableEscapeDoorGenerator].TagsToActivate);
        }

        private void UnsubscribeEnableEscapeDoorGeneratorHandler(SOQuest quest) { }

        private void OnEnableEscapeDoorGeneratorCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeEnableEscapeDoorGeneratorHandler(quest);
            ShowCompletedMessage(quest);
            if (!_completeAudioAlreadyPlayed[(int)QuestsEnum.EnableEscapeDoorGenerator])
            {
                PlayLine(quest, 1, true);
                _completeAudioAlreadyPlayed[(int)QuestsEnum.EnableEscapeDoorGenerator] = true;
            }
            StartCoroutine(TimeToReactivateGeneratorCoroutine(quest));
        }
        #endregion

        #region Escape Quest
        private void OnEscapeAdded(SOQuest quest)
        {
            UnsubscribeEscapeHandler(quest);
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            _onUnsubscribeQuest[(int)QuestsEnum.Escape] = UnsubscribeEscapeHandler;
            if (!_addAudioAlreadyPlayed[(int)QuestsEnum.Escape])
            {
                PlayLine(quest, 0);
                _addAudioAlreadyPlayed[(int)QuestsEnum.Escape] = true;
            }
        }

        private void UnsubscribeEscapeHandler(SOQuest quest) { }

        private void OnEscapeCompleted(SOQuest quest)
        {
            if (switchLevelEvent != null)
                switchLevelEvent.RaiseEvent(this, destinationScene);
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeEscapeHandler(quest);
            ShowCompletedMessage(quest);
            if (!_completeAudioAlreadyPlayed[(int)QuestsEnum.Escape])
            {
                PlayLine(quest, 1, true);
                _completeAudioAlreadyPlayed[(int)QuestsEnum.Escape] = true;
            }
        }
        #endregion

    }
}
