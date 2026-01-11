using System.Collections;
using UnityEngine;

namespace EchoCity
{
    public class TutorialQuestManager : QuestsManager
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

            _onAddQuest[(int)QuestsEnum.Sprint_Tutorial] = OnSprint_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.Sprint_Tutorial] = OnSprint_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.Jump_Tutorial] = OnJump_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.Jump_Tutorial] = OnJump_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.Interact_Tutorial] = OnInteract_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.Interact_Tutorial] = OnInteract_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.PickUp_Tutorial] = OnPickUp_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.PickUp_Tutorial] = OnPickUp_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.OpenInventory_Tutorial] = OnOpenInventory_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.OpenInventory_Tutorial] = OnOpenInventory_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.EquipItem_Tutorial] = OnEquipItem_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.EquipItem_Tutorial] = OnEquipItem_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.UseItem_Tutorial] = OnUseItem_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.UseItem_Tutorial] = OnUseItem_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.DropItem_Tutorial] = OnDropItem_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.DropItem_Tutorial] = OnDropItem_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.SwitchLightsOff_Tutorial] = OnSwitchLightsOff_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.SwitchLightsOff_Tutorial] = OnSwitchLightsOff_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.UseLowSO_Tutorial] = OnUseLowSO_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.UseLowSO_Tutorial] = OnUseLowSO_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.UseMidSO_Tutorial] = OnUseMidSO_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.UseMidSO_Tutorial] = OnUseMidSO_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.UseHighSO_Tutorial] = OnUseHighSO_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.UseHighSO_Tutorial] = OnUseHighSO_TutorialCompleted;

            _onAddQuest[(int)QuestsEnum.AttractEnemy_Tutorial] = OnEnemySoundChase_TutorialAdded;
            _onCompleteQuest[(int)QuestsEnum.AttractEnemy_Tutorial] = OnEnemySoundChase_TutorialCompleted;

            //tests
            _onAddQuest[(int)QuestsEnum.FindWalkieTalkie] = OnFindWalkieTalkieAdded;
            _onCompleteQuest[(int)QuestsEnum.FindWalkieTalkie] = OnFindWalkieTalkieCompleted;
        }

        #region TestOne
        private void OnTestOneAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += TestQuestHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.TestOne] = UnsubscribeTestOneHandler;
        }

        private void UnsubscribeTestOneHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= TestQuestHandler;
            }
        }

        private void TestQuestHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemAdded || pickable != PickablesEnum.CannedFood)
                return;

            IncrementQuestProgress(QuestsEnum.TestOne);
            if (questProgression[(int)QuestsEnum.TestOne] < activeQuests[(int)QuestsEnum.TestOne].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Phone_Picked, true) });
        }

        private void OnTestOneCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeTestOneHandler(quest);
        }
        #endregion

        #region TestTwo
        private void OnTestTwoAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += TestTwoQuestHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.TestTwo] = UnsubscribeTestTwoHandler;
        }

        private void UnsubscribeTestTwoHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= TestTwoQuestHandler;
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
            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.KaelID_Picked, true) });
        }

        private void OnTestTwoCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeTestTwoHandler(quest);
        }
        #endregion

        #region Move_Tutorial
        private void OnMove_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest Move_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised += MoveTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.Move_Tutorial] = UnsubscribeMoveTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeMoveTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised -= MoveTutorialHandler;
            }
        }

        private void MoveTutorialHandler(IEventSender sender, MovementCodeEnum code)
        {
            if (code != MovementCodeEnum.Move)
                return;

            IncrementQuestProgress(QuestsEnum.Move_Tutorial);
            if (questProgression[(int)QuestsEnum.Move_Tutorial] < activeQuests[(int)QuestsEnum.Move_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Move_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.Move_Tutorial]);
        }

        private void OnMove_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeMoveTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region Look_Tutorial
        private void OnLook_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest Look_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised += LookTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.Look_Tutorial] = UnsubscribeLookTutorialHandler;
        }

        private void UnsubscribeLookTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised -= LookTutorialHandler;
            }
        }

        private void LookTutorialHandler(IEventSender sender, MovementCodeEnum code)
        {
            if (code != MovementCodeEnum.Look)
                return;

            IncrementQuestProgress(QuestsEnum.Look_Tutorial);
            if (questProgression[(int)QuestsEnum.Look_Tutorial] < activeQuests[(int)QuestsEnum.Look_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Look_TutorialCompleted, true) });
        }

        private void OnLook_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeLookTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region Sprint_Tutorial
        private void OnSprint_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest Sprint_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised += SprintTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.Sprint_Tutorial] = UnsubscribeSprintTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeSprintTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised -= SprintTutorialHandler;
            }
        }

        private void SprintTutorialHandler(IEventSender sender, MovementCodeEnum code)
        {
            if (code != MovementCodeEnum.Sprint)
                return;

            IncrementQuestProgress(QuestsEnum.Sprint_Tutorial);
            if (questProgression[(int)QuestsEnum.Sprint_Tutorial] < activeQuests[(int)QuestsEnum.Sprint_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Sprint_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.Sprint_Tutorial]);
        }

        private void OnSprint_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeSprintTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region Jump_Tutorial
        private void OnJump_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest Jump_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised += JumpTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.Jump_Tutorial] = UnsubscribeJumpTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeJumpTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised -= JumpTutorialHandler;
            }
        }

        private void JumpTutorialHandler(IEventSender sender, MovementCodeEnum code)
        {
            if (code != MovementCodeEnum.Jump)
                return;

            IncrementQuestProgress(QuestsEnum.Jump_Tutorial);
            if (questProgression[(int)QuestsEnum.Jump_Tutorial] < activeQuests[(int)QuestsEnum.Jump_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Jump_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.Jump_Tutorial]);
        }

        private void OnJump_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeJumpTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region Interact_Tutorial
        private void OnInteract_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest Interact_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised += InteractTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.Interact_Tutorial] = UnsubscribeInteractTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeInteractTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised -= InteractTutorialHandler;
            }
        }

        private void InteractTutorialHandler(IEventSender sender, InteractionEnum code)
        {
            if (code == InteractionEnum.Pickable)
                return;

            IncrementQuestProgress(QuestsEnum.Interact_Tutorial);
            if (questProgression[(int)QuestsEnum.Interact_Tutorial] < activeQuests[(int)QuestsEnum.Interact_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Interact_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.Interact_Tutorial]);
        }

        private void OnInteract_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeInteractTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region PickUp_Tutorial
        private void OnPickUp_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest PickUp_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised += PickUpTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.PickUp_Tutorial] = UnsubscribePickUpTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribePickUpTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised -= PickUpTutorialHandler;
            }
        }

        private void PickUpTutorialHandler(IEventSender sender, InteractionEnum code)
        {
            if (code != InteractionEnum.Pickable)
                return;

            IncrementQuestProgress(QuestsEnum.PickUp_Tutorial);
            if (questProgression[(int)QuestsEnum.PickUp_Tutorial] < activeQuests[(int)QuestsEnum.PickUp_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.PickUp_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.PickUp_Tutorial]);
        }

        private void OnPickUp_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribePickUpTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region OpenInventory_Tutorial

        private void OnOpenInventory_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest OpenInventory_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ShowUI)
                    ((SOShowUIEvent)eventBase).OnEventRaised += OpenInventoryTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.OpenInventory_Tutorial] = UnsubscribeOpenInventoryTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeOpenInventoryTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ShowUI)
                    ((SOShowUIEvent)eventBase).OnEventRaised -= OpenInventoryTutorialHandler;
            }
        }

        private void OpenInventoryTutorialHandler(IEventSender sender, ShowableUIEnum uiElement, EventParams eventParams)
        {
            if (uiElement != ShowableUIEnum.HUD)
                return;
            if (!(eventParams is HudParams @param) || @param.HudState != HudEnum.Inventory)
                return;

            IncrementQuestProgress(QuestsEnum.OpenInventory_Tutorial);
            if (questProgression[(int)QuestsEnum.OpenInventory_Tutorial] < activeQuests[(int)QuestsEnum.OpenInventory_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.OpenInventory_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.OpenInventory_Tutorial]);
        }

        private void OnOpenInventory_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeOpenInventoryTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region EquipItem_Tutorial
        private void OnEquipItem_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest EquipItem_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.EquippedItemChanged)
                    ((SOEquippedItemChangedEvent)eventBase).OnEventRaised += EquipItemTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.EquipItem_Tutorial] = UnsubscribeEquipItemTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeEquipItemTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.EquippedItemChanged)
                    ((SOEquippedItemChangedEvent)eventBase).OnEventRaised -= EquipItemTutorialHandler;
            }
        }

        private void EquipItemTutorialHandler(IEventSender sender, PickablesEnum equippedItem)
        {
            if (equippedItem == PickablesEnum.Hands)
                return;

            IncrementQuestProgress(QuestsEnum.EquipItem_Tutorial);
            if (questProgression[(int)QuestsEnum.EquipItem_Tutorial] < activeQuests[(int)QuestsEnum.EquipItem_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.EquipItem_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.EquipItem_Tutorial]);
        }

        private void OnEquipItem_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeEquipItemTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region UseItem_Tutorial
        private void OnUseItem_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest UseItem_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ItemUsed)
                    ((SOItemUsedEvent)eventBase).OnEventRaised += UseItemTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.UseItem_Tutorial] = UnsubscribeUseItemTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeUseItemTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ItemUsed)
                    ((SOItemUsedEvent)eventBase).OnEventRaised -= UseItemTutorialHandler;
            }
        }

        private void UseItemTutorialHandler(IEventSender sender, SOPickable toolUsed)
        {
            IncrementQuestProgress(QuestsEnum.UseItem_Tutorial);
            if (questProgression[(int)QuestsEnum.UseItem_Tutorial] < activeQuests[(int)QuestsEnum.UseItem_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.UseItem_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.UseItem_Tutorial]);
        }

        private void OnUseItem_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeUseItemTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region DropItem_Tutorial
        private void OnDropItem_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest DropItem_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += DropItemTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.DropItem_Tutorial] = UnsubscribeDropItemTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeDropItemTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= DropItemTutorialHandler;
            }
        }

        private void DropItemTutorialHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemDropped)
                return;

            IncrementQuestProgress(QuestsEnum.DropItem_Tutorial);
            if (questProgression[(int)QuestsEnum.DropItem_Tutorial] < activeQuests[(int)QuestsEnum.DropItem_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.DropItem_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.DropItem_Tutorial]);
        }

        private void OnDropItem_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeDropItemTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region SwitchLightsOff_Tutorial
        private void OnSwitchLightsOff_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest SwitchLightsOff_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.EchoMaterialUpdated)
                    ((SOEchoMaterialUpdated)eventBase).OnEventRaised += SwitchLightsOffTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.SwitchLightsOff_Tutorial] = UnsubscribeSwitchLightsOffTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeSwitchLightsOffTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.EchoMaterialUpdated)
                    ((SOEchoMaterialUpdated)eventBase).OnEventRaised -= SwitchLightsOffTutorialHandler;
            }
        }

        private void SwitchLightsOffTutorialHandler(IEventSender sender, bool active)
        {
            if (!active)
                return;

            IncrementQuestProgress(QuestsEnum.SwitchLightsOff_Tutorial);
            if (questProgression[(int)QuestsEnum.SwitchLightsOff_Tutorial] < activeQuests[(int)QuestsEnum.SwitchLightsOff_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.SwitchLightsOff_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.SwitchLightsOff_Tutorial]);
        }

        private void OnSwitchLightsOff_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeSwitchLightsOffTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region UseLowSO_Tutorial

        private void OnUseLowSO_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest UseLowSO_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ItemUsed)
                    ((SOItemUsedEvent)eventBase).OnEventRaised += UseLowSOTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.UseLowSO_Tutorial] = UnsubscribeUseLowSOTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeUseLowSOTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ItemUsed)
                    ((SOItemUsedEvent)eventBase).OnEventRaised -= UseLowSOTutorialHandler;
            }
        }

        private void UseLowSOTutorialHandler(IEventSender sender, SOPickable toolUsed)
        {
            if (toolUsed.ToolSound.SoundClass.Frequency != Frequency.Low)
                return;

            IncrementQuestProgress(QuestsEnum.UseLowSO_Tutorial);
            if (questProgression[(int)QuestsEnum.UseLowSO_Tutorial] < activeQuests[(int)QuestsEnum.UseLowSO_Tutorial].CountToComplete)
                return;
            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.UseLowSO_TutorialCompleted, true) });
            CompleteQuest(activeQuests[(int)QuestsEnum.UseLowSO_Tutorial]);
        }

        private void OnUseLowSO_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeUseLowSOTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region UseMidSO_Tutorial
        private void OnUseMidSO_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest UseMidSO_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ItemUsed)
                    ((SOItemUsedEvent)eventBase).OnEventRaised += UseMidSOTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.UseMidSO_Tutorial] = UnsubscribeUseMidSOTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeUseMidSOTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ItemUsed)
                    ((SOItemUsedEvent)eventBase).OnEventRaised -= UseMidSOTutorialHandler;
            }
        }

        private void UseMidSOTutorialHandler(IEventSender sender, SOPickable toolUsed)
        {
            if (toolUsed.ToolSound.SoundClass.Frequency != Frequency.Mid)
                return;

            IncrementQuestProgress(QuestsEnum.UseMidSO_Tutorial);
            if (questProgression[(int)QuestsEnum.UseMidSO_Tutorial] < activeQuests[(int)QuestsEnum.UseMidSO_Tutorial].CountToComplete)
                return;
            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.UseMidSO_TutorialCompleted, true) });
            CompleteQuest(activeQuests[(int)QuestsEnum.UseMidSO_Tutorial]);
        }

        private void OnUseMidSO_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeUseMidSOTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region UseHighSO_Tutorial
        private void OnUseHighSO_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest UseHighSO_Tutorial added.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ItemUsed)
                    ((SOItemUsedEvent)eventBase).OnEventRaised += UseHighSOTutorialHandler;
            }
            _onUnsubscribeQuest[(int)QuestsEnum.UseHighSO_Tutorial] = UnsubscribeUseHighSOTutorialHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeUseHighSOTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ItemUsed)
                    ((SOItemUsedEvent)eventBase).OnEventRaised -= UseHighSOTutorialHandler;
            }
        }

        private void UseHighSOTutorialHandler(IEventSender sender, SOPickable toolUsed)
        {
            if (toolUsed.ToolSound.SoundClass.Frequency != Frequency.High)
                return;

            IncrementQuestProgress(QuestsEnum.UseHighSO_Tutorial);
            if (questProgression[(int)QuestsEnum.UseHighSO_Tutorial] < activeQuests[(int)QuestsEnum.UseHighSO_Tutorial].CountToComplete)
                return;
            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.UseHighSO_TutorialCompleted, true) });
            CompleteQuest(activeQuests[(int)QuestsEnum.UseHighSO_Tutorial]);
        }

        private void OnUseHighSO_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeUseHighSOTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion

        #region EnemySoundChase_Tutorial
        private void OnEnemySoundChase_TutorialAdded(SOQuest quest)
        {
            Log.DLazy(() => $"Quest EnemySoundChase_Tutorial added.", this);
            StartCoroutine(StartEnemyQuestDelay(quest));
        }

        private void UnsubscribeEnemySoundChaseTutorialHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.EnemyStateTransition)
                    ((SOEnemyStateTransitionEvent)eventBase).OnEventRaised -= EnemySoundChaseTutorialHandler;
            }
        }

        private void EnemySoundChaseTutorialHandler(IEventSender sender, EnemyStateEnum newState, EnemyStateEnum previousState, Transform enemyTransform)
        {
            if (newState != EnemyStateEnum.SoundChase)
                return;

            IncrementQuestProgress(QuestsEnum.AttractEnemy_Tutorial);
            if (questProgression[(int)QuestsEnum.AttractEnemy_Tutorial] < activeQuests[(int)QuestsEnum.AttractEnemy_Tutorial].CountToComplete)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.EnemySoundChase_TutorialCompleted, true), new PuzzleTagState(PuzzleTagEnum.TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.AttractEnemy_Tutorial]);
        }

        private void OnEnemySoundChase_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeEnemySoundChaseTutorialHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }

        private IEnumerator StartEnemyQuestDelay(SOQuest quest)
        {
            questsUpdatedEvent.RaiseEvent(this, 0, (int)QuestStateEnum.ResetQuestsManager);
            yield return new WaitForSeconds(activeQuests[(int)QuestsEnum.UseHighSO_Tutorial].ScriptContainer.DialogLines[1].AudioClip.length);
            PlayLine(quest, 0);
            yield return new WaitForSeconds(activeQuests[(int)QuestsEnum.AttractEnemy_Tutorial].ScriptContainer.DialogLines[0].AudioClip.length);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.EnemyStateTransition)
                    ((SOEnemyStateTransitionEvent)eventBase).OnEventRaised += EnemySoundChaseTutorialHandler;
            }
            questsUpdatedEvent.RaiseEvent(this, (int)QuestsEnum.AttractEnemy_Tutorial, 0);
            _onUnsubscribeQuest[(int)QuestsEnum.AttractEnemy_Tutorial] = UnsubscribeEnemySoundChaseTutorialHandler;
        }
        #endregion

        #region FindWalkieTalkie

        private void OnFindWalkieTalkieAdded(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                {
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= DropWalkieTalkieHandler;
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += FindWalkieTalkieHandler;
                }
            }
            Log.DLazy(() => $"Quest FindWalkieTalkie added.", this);

            _onUnsubscribeQuest[(int)QuestsEnum.FindWalkieTalkie] = UnsubscribeFindWalkieTalkieHandler;
            PlayLine(quest, 0);
        }

        private void UnsubscribeFindWalkieTalkieHandler(SOQuest quest)
        {
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                {

                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= FindWalkieTalkieHandler;
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised += DropWalkieTalkieHandler;
                }
            }
        }

        private void FindWalkieTalkieHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemAdded || pickable != PickablesEnum.WalkieTalkie)
                return;

            IncrementQuestProgress(QuestsEnum.FindWalkieTalkie);
            if (questProgression[(int)QuestsEnum.FindWalkieTalkie] < activeQuests[(int)QuestsEnum.FindWalkieTalkie].CountToComplete)
                return;
            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.WalkieTalkie_Picked, true) });
            CompleteQuest(activeQuests[(int)QuestsEnum.FindWalkieTalkie]);
        }

        private void DropWalkieTalkieHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemDropped || pickable != PickablesEnum.WalkieTalkie)
                return;

            _puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.WalkieTalkie_Picked, false) }, true);
        }

        private void OnFindWalkieTalkieCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            UnsubscribeFindWalkieTalkieHandler(quest);
            ShowCompletedMessage(quest);
            PlayLine(quest, 1, true);
        }
        #endregion  

    }
}