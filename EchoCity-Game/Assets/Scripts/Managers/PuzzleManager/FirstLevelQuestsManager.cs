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
            PlayLine(quest, 0);
        }

        private void MoveTutorialHandler(IEventSender sender, MovementCodeEnum code)
        {
            if (code != MovementCodeEnum.Move)
                return;

            IncrementQuestProgress(QuestsEnum.Move_Tutorial);
            if (questProgression[(int)QuestsEnum.Move_Tutorial] < activeQuests[(int)QuestsEnum.Move_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Move_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.Move_Tutorial]);
        }

        private void OnMove_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised -= MoveTutorialHandler;
            }
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
        }

        private void LookTutorialHandler(IEventSender sender, MovementCodeEnum code)
        {
            if (code != MovementCodeEnum.Look)
                return;

            IncrementQuestProgress(QuestsEnum.Look_Tutorial);
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
            PlayLine(quest, 0);
        }

        private void SprintTutorialHandler(IEventSender sender, MovementCodeEnum code)
        {
            if (code != MovementCodeEnum.Sprint)
                return;

            IncrementQuestProgress(QuestsEnum.Sprint_Tutorial);
            if (questProgression[(int)QuestsEnum.Sprint_Tutorial] < activeQuests[(int)QuestsEnum.Sprint_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Sprint_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.Sprint_Tutorial]);
        }

        private void OnSprint_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised -= SprintTutorialHandler;
            }

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
            PlayLine(quest, 0);
        }

        private void JumpTutorialHandler(IEventSender sender, MovementCodeEnum code)
        {
            if (code != MovementCodeEnum.Jump)
                return;

            IncrementQuestProgress(QuestsEnum.Jump_Tutorial);
            if (questProgression[(int)QuestsEnum.Jump_Tutorial] < activeQuests[(int)QuestsEnum.Jump_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Jump_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.Jump_Tutorial]);
        }

        private void OnJump_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.PlayerMovement)
                    ((SOPlayerMovementEvent)eventBase).OnEventRaised -= JumpTutorialHandler;
            }
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
            PlayLine(quest, 0);
        }

        private void InteractTutorialHandler(IEventSender sender, InteractionEnum code)
        {
            if (code != InteractionEnum.Interactable)
                return;

            IncrementQuestProgress(QuestsEnum.Interact_Tutorial);
            if (questProgression[(int)QuestsEnum.Interact_Tutorial] < activeQuests[(int)QuestsEnum.Interact_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.Interact_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.Interact_Tutorial]);
        }

        private void OnInteract_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised -= InteractTutorialHandler;
            }
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
            PlayLine(quest, 0);
        }

        private void PickUpTutorialHandler(IEventSender sender, InteractionEnum code)
        {
            if (code != InteractionEnum.Pickable)
                return;

            IncrementQuestProgress(QuestsEnum.PickUp_Tutorial);
            if (questProgression[(int)QuestsEnum.PickUp_Tutorial] < activeQuests[(int)QuestsEnum.PickUp_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.PickUp_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.PickUp_Tutorial]);
        }

        private void OnPickUp_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.Interaction)
                    ((SOInteractionEvent)eventBase).OnEventRaised -= PickUpTutorialHandler;
            }
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
            PlayLine(quest, 0);
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

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.OpenInventory_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.OpenInventory_Tutorial]);
        }

        private void OnOpenInventory_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ShowUI)
                    ((SOShowUIEvent)eventBase).OnEventRaised -= OpenInventoryTutorialHandler;
            }
            // PlayLine(quest, 1, true);
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
            // PlayLine(quest, 0);
        }

        private void EquipItemTutorialHandler(IEventSender sender, PickablesEnum equippedItem)
        {
            if (equippedItem == PickablesEnum.Hands)
                return;

            IncrementQuestProgress(QuestsEnum.EquipItem_Tutorial);
            if (questProgression[(int)QuestsEnum.EquipItem_Tutorial] < activeQuests[(int)QuestsEnum.EquipItem_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.EquipItem_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.EquipItem_Tutorial]);
        }

        private void OnEquipItem_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.EquippedItemChanged)
                    ((SOEquippedItemChangedEvent)eventBase).OnEventRaised -= EquipItemTutorialHandler;
            }
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
            PlayLine(quest, 0);
        }

        private void UseItemTutorialHandler(IEventSender sender, SOPickable toolUsed)
        {
            IncrementQuestProgress(QuestsEnum.UseItem_Tutorial);
            if (questProgression[(int)QuestsEnum.UseItem_Tutorial] < activeQuests[(int)QuestsEnum.UseItem_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.UseItem_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.UseItem_Tutorial]);
        }

        private void OnUseItem_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.ItemUsed)
                    ((SOItemUsedEvent)eventBase).OnEventRaised -= UseItemTutorialHandler;
            }
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
            PlayLine(quest, 0);
        }

        private void DropItemTutorialHandler(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (inventoryCodes != InventoryCodesEnum.ItemDropped)
                return;

            IncrementQuestProgress(QuestsEnum.DropItem_Tutorial);
            if (questProgression[(int)QuestsEnum.DropItem_Tutorial] < activeQuests[(int)QuestsEnum.DropItem_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.DropItem_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.DropItem_Tutorial]);
        }

        private void OnDropItem_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.InventoryChanged)
                    ((SOInventoryChangedEvent)eventBase).OnEventRaised -= DropItemTutorialHandler;
            }
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
            PlayLine(quest, 0);
        }

        private void SwitchLightsOffTutorialHandler(IEventSender sender, bool active)
        {
            if (!active)
                return;

            IncrementQuestProgress(QuestsEnum.SwitchLightsOff_Tutorial);
            if (questProgression[(int)QuestsEnum.SwitchLightsOff_Tutorial] < activeQuests[(int)QuestsEnum.SwitchLightsOff_Tutorial].CountToComplete)
                return;

            puzzleManager.SetTags(new PuzzleTagState[] { new PuzzleTagState(PuzzleTagEnum.SwitchLightsOff_TutorialCompleted, true) }, false);
            CompleteQuest(activeQuests[(int)QuestsEnum.SwitchLightsOff_Tutorial]);
        }

        private void OnSwitchLightsOff_TutorialCompleted(SOQuest quest)
        {
            Log.DLazy(() => $"Quest {quest.name} completed.", this);
            foreach (var eventBase in quest.SubscribeToEvents)
            {
                if (eventBase.EventType == EchoCityEventsEnum.EchoMaterialUpdated)
                    ((SOEchoMaterialUpdated)eventBase).OnEventRaised -= SwitchLightsOffTutorialHandler;
            }
            PlayLine(quest, 1, true);
        }
        #endregion



    }
}