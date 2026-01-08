using System;
using UnityEngine;


namespace EchoCity
{
    public class EventsLogger : MonoBehaviour
    {

#if UNITY_EDITOR
        public static class SenderColorCache
        {
            private static class Storage<T>
            {
                public static readonly string HexColor = GenerateColor(typeof(T).Name);

                private static string GenerateColor(string name)
                {
                    int hash = name.GetHashCode();
                    float r = (Mathf.Abs(hash & 0xFF0000) >> 16) / 255f;
                    float g = (Mathf.Abs(hash & 0x00FF00) >> 8) / 255f;
                    float b = (Mathf.Abs(hash & 0x0000FF) >> 0) / 255f;

                    UnityEngine.Color c = UnityEngine.Color.Lerp(new UnityEngine.Color(r, g, b), UnityEngine.Color.white, 0.4f);
                    return "#" + ColorUtility.ToHtmlStringRGB(c);
                }
            }

            public static string GetColor(object sender)
            {
                Type type = sender.GetType();
                var storageType = typeof(Storage<>).MakeGenericType(type);
                return (string)storageType.GetField("HexColor").GetValue(null);
            }
        }

#pragma warning disable CS0414
        [TextArea(1, 2)][SerializeField] private string description = "Logs all concrete events with their payloads";
#pragma warning restore CS0414

        [Header("Events")]
        [SerializeField] private SOAttractionInfoEvent attractionInfoEvent;
        [SerializeField] private bool logAttractionInfo = true;

        [SerializeField] private SOEchoMaterialUpdated echoMaterialUpdatedEvent;
        [SerializeField] private bool logEchoMaterialUpdated = true;

        [SerializeField] private SOEnemyStateTransitionEvent enemyStateTransitionEvent;
        [SerializeField] private bool logEnemyStateTransition = true;

        [SerializeField] private SOEquippedItemChangedEvent equippedItemChangedEvent;
        [SerializeField] private bool logEquippedItemChanged = true;

        [SerializeField] private SOGameManagerStateTransitionEvent gameManagerStateTransitionEvent;
        [SerializeField] private bool logGameManagerStateTransition = true;

        [SerializeField] private SOInteractionEvent interactionEvent;
        [SerializeField] private bool logInteraction = true;

        [SerializeField] private SOInventoryChangedEvent inventoryChangedEvent;
        [SerializeField] private bool logInventoryChanged = true;

        [SerializeField] private SOItemUsedEvent itemUsedEvent;
        [SerializeField] private bool logItemUsed = true;

        [SerializeField] private SOLevelActionEvent levelActionEvent;
        [SerializeField] private bool logLevelAction = true;

        [SerializeField] private SOPlayerInputEvent playerInputEvent;
        [SerializeField] private bool logPlayerInput = true;

        [SerializeField] private SOPlayerMovementEvent playerMovementEvent;
        [SerializeField] private bool logPlayerMovement = true;

        [SerializeField] private SOQuestUpdatedEvent questUpdatedEvent;
        [SerializeField] private bool logQuestUpdated = true;

        [SerializeField] private SOSceneLoaderTriggerEvent sceneLoaderTriggerEvent;
        [SerializeField] private bool logSceneLoaderTrigger = true;

        [SerializeField] private SOSetMaterialEvent setMaterialEvent;
        [SerializeField] private bool logSetMaterial = true;

        [SerializeField] private SOSetPlayerOnSpawnEvent setPlayerOnSpawnEvent;
        [SerializeField] private bool logSetPlayerOnSpawn = true;

        [SerializeField] private SOShowInteractionEvent showInteractionEvent;
        [SerializeField] private bool logShowInteraction = true;

        [SerializeField] private SOShowUIEvent showUIEvent;
        [SerializeField] private bool logShowUI = true;

        [SerializeField] private SOSoundEmittedEvent soundEmittedEvent;
        [SerializeField] private bool logSoundEmitted = true;

        [SerializeField] private SOSubmitFeedbackEvent submitFeedbackEvent;
        [SerializeField] private bool logSubmitFeedback = true;

        [SerializeField] private SOSwitchLevelEvent switchLevelEvent;
        [SerializeField] private bool logSwitchLevel = true;

        [SerializeField] private SOSwitchToGameStateEvent switchToGameStateEvent;
        [SerializeField] private bool logSwitchToGameState = true;

        [SerializeField] private SOTimerEvent timerEvent;
        [SerializeField] private bool logTimerEvent = true;


        [SerializeField] private SOUITriggerEvent uiTriggerEvent;
        [SerializeField] private bool logUITrigger = true;

        [SerializeField] private bool activateDeactivateAll = true;

        void OnEnable()
        {
            RegisterEvents();
        }

        void OnDisable()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            if (attractionInfoEvent != null) attractionInfoEvent.OnEventRaised += OnAttractionInfo;
            if (echoMaterialUpdatedEvent != null) echoMaterialUpdatedEvent.OnEventRaised += OnEchoMaterialUpdated;
            if (enemyStateTransitionEvent != null) enemyStateTransitionEvent.OnEventRaised += OnEnemyStateTransition;
            if (equippedItemChangedEvent != null) equippedItemChangedEvent.OnEventRaised += OnEquippedItemChanged;
            if (gameManagerStateTransitionEvent != null) gameManagerStateTransitionEvent.OnEventRaised += OnGameManagerStateTransition;
            if (interactionEvent != null) interactionEvent.OnEventRaised += OnInteraction;
            if (inventoryChangedEvent != null) inventoryChangedEvent.OnEventRaised += OnInventoryChanged;
            if (itemUsedEvent != null) itemUsedEvent.OnEventRaised += OnItemUsed;
            if (levelActionEvent != null) levelActionEvent.OnEventRaised += OnLevelAction;
            if (playerInputEvent != null) playerInputEvent.OnEventRaised += OnPlayerInput;
            if (playerMovementEvent != null) playerMovementEvent.OnEventRaised += OnPlayerMovement;
            if (questUpdatedEvent != null) questUpdatedEvent.OnEventRaised += OnQuestUpdated;
            if (sceneLoaderTriggerEvent != null) sceneLoaderTriggerEvent.OnEventRaised += OnSceneLoaderTrigger;
            if (setMaterialEvent != null) setMaterialEvent.OnEventRaised += OnSetMaterial;
            if (setPlayerOnSpawnEvent != null) setPlayerOnSpawnEvent.OnEventRaised += OnSetPlayerOnSpawn;
            if (showInteractionEvent != null) showInteractionEvent.OnEventRaised += OnShowInteraction;
            if (showUIEvent != null) showUIEvent.OnEventRaised += OnShowUI;
            if (soundEmittedEvent != null) soundEmittedEvent.OnEventRaised += OnSoundEmitted;
            if (submitFeedbackEvent != null) submitFeedbackEvent.OnEventRaised += OnSubmitFeedback;
            if (switchLevelEvent != null) switchLevelEvent.OnEventRaised += OnSwitchLevel;
            if (switchToGameStateEvent != null) switchToGameStateEvent.OnEventRaised += OnSwitchToGameState;
            if (timerEvent != null) timerEvent.OnEventRaised += OnTimerEvent;
            if (uiTriggerEvent != null) uiTriggerEvent.OnEventRaised += OnUITrigger;
        }

        private void UnregisterEvents()
        {
            if (attractionInfoEvent != null) attractionInfoEvent.OnEventRaised -= OnAttractionInfo;
            if (echoMaterialUpdatedEvent != null) echoMaterialUpdatedEvent.OnEventRaised -= OnEchoMaterialUpdated;
            if (enemyStateTransitionEvent != null) enemyStateTransitionEvent.OnEventRaised -= OnEnemyStateTransition;
            if (equippedItemChangedEvent != null) equippedItemChangedEvent.OnEventRaised -= OnEquippedItemChanged;
            if (gameManagerStateTransitionEvent != null) gameManagerStateTransitionEvent.OnEventRaised -= OnGameManagerStateTransition;
            if (interactionEvent != null) interactionEvent.OnEventRaised -= OnInteraction;
            if (inventoryChangedEvent != null) inventoryChangedEvent.OnEventRaised -= OnInventoryChanged;
            if (itemUsedEvent != null) itemUsedEvent.OnEventRaised -= OnItemUsed;
            if (levelActionEvent != null) levelActionEvent.OnEventRaised -= OnLevelAction;
            if (playerInputEvent != null) playerInputEvent.OnEventRaised -= OnPlayerInput;
            if (playerMovementEvent != null) playerMovementEvent.OnEventRaised -= OnPlayerMovement;
            if (questUpdatedEvent != null) questUpdatedEvent.OnEventRaised -= OnQuestUpdated;
            if (sceneLoaderTriggerEvent != null) sceneLoaderTriggerEvent.OnEventRaised -= OnSceneLoaderTrigger;
            if (setMaterialEvent != null) setMaterialEvent.OnEventRaised -= OnSetMaterial;
            if (setPlayerOnSpawnEvent != null) setPlayerOnSpawnEvent.OnEventRaised -= OnSetPlayerOnSpawn;
            if (showInteractionEvent != null) showInteractionEvent.OnEventRaised -= OnShowInteraction;
            if (showUIEvent != null) showUIEvent.OnEventRaised -= OnShowUI;
            if (soundEmittedEvent != null) soundEmittedEvent.OnEventRaised -= OnSoundEmitted;
            if (submitFeedbackEvent != null) submitFeedbackEvent.OnEventRaised -= OnSubmitFeedback;
            if (switchLevelEvent != null) switchLevelEvent.OnEventRaised -= OnSwitchLevel;
            if (switchToGameStateEvent != null) switchToGameStateEvent.OnEventRaised -= OnSwitchToGameState;
            if (timerEvent != null) timerEvent.OnEventRaised -= OnTimerEvent;
            if (uiTriggerEvent != null) uiTriggerEvent.OnEventRaised -= OnUITrigger;
        }

        private void OnGameManagerStateTransition(IEventSender sender, GameStatesEnum from, GameStatesEnum to)
        {
            if (!logGameManagerStateTransition) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: GameManager state transition {from} -> {to}", this);
        }

        private void OnEquippedItemChanged(IEventSender sender, PickablesEnum newEquippedItem)
        {
            if (!logEquippedItemChanged) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Equipped item changed to {newEquippedItem}", this);
        }

        private void OnSetPlayerOnSpawn(IEventSender sender)
        {
            if (!logSetPlayerOnSpawn) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Set player on spawn", this);
        }

        private void OnQuestUpdated(IEventSender sender, int questIndex, int code)
        {
            if (!logQuestUpdated) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Quest updated (index: {questIndex}, code: {code})", this);
        }

        private void OnSceneLoaderTrigger(IEventSender sender, SceneLoaderTriggerEnum triggerCode)
        {
            if (!logSceneLoaderTrigger) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Scene loader trigger | code: {triggerCode}", this);
        }

        private void OnShowInteraction(IEventSender sender, bool isRaycastInteractable, bool showDescription, string description)
        {
            if (!logShowInteraction) return;
            string desc = string.IsNullOrEmpty(description) ? "<empty>" : description;
            Log.DLazy(() => $"{GetColoredName(sender)}: Show interaction | raycast: {isRaycastInteractable} | showDescription: {showDescription} | text: {desc}", this);
        }

        private void OnPlayerInput(IEventSender sender, InputEnum inputType, bool activate)
        {
            if (!logPlayerInput) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Player input | type: {inputType} | activate: {activate}", this);
        }

        private void OnLevelAction(IEventSender sender, LevelActionCodeEnum code, SceneEnum scene)
        {
            if (!logLevelAction) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Level action | code: {code} | scene: {scene}", this);
        }

        private void OnShowUI(IEventSender sender, ShowableUIEnum ui, EventParams eventParams)
        {
            if (!logShowUI) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Show UI | ui: {ui} | params: {DescribeEventParams(eventParams)}", this);
        }

        private void OnInventoryChanged(IEventSender sender, PickablesEnum pickable, PickableTypeEnum pickableType, InventoryCodesEnum inventoryCodes)
        {
            if (!logInventoryChanged) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Inventory changed | pickable: {pickable} ({pickableType}) | code: {inventoryCodes}", this);
        }

        private void OnAttractionInfo(IEventSender sender, IAttraction attraction, Transform targetTransform, bool aboveThreshold)
        {
            if (!logAttractionInfo) return;
            string attractionInfo = attraction != null ? attraction.ToString() : "null";
            string transformInfo = DescribeTransform(targetTransform);
            Log.DLazy(() => $"{GetColoredName(sender)}: Attraction info | attraction: {attractionInfo} | target: {transformInfo} | aboveThreshold: {aboveThreshold}", this);
        }

        private void OnSetMaterial(IEventSender sender, EchoMaterialCodeEnum code)
        {
            if (!logSetMaterial) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Set material | code: {code}", this);
        }

        private void OnSwitchToGameState(IEventSender sender, GameStatesEnum gameState, EventParams eventParams)
        {
            if (!logSwitchToGameState) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Switch to game state | state: {gameState} | params: {DescribeEventParams(eventParams)}", this);
        }

        private void OnSwitchLevel(IEventSender sender, SceneEnum level, EventParams @params)
        {
            if (!logSwitchLevel) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Switch level | level: {level} | params: {DescribeEventParams(@params)}", this);
        }

        private void OnSubmitFeedback(IEventSender sender, int rating, string feedback)
        {
            if (!logSubmitFeedback) return;
            string text = string.IsNullOrEmpty(feedback) ? "<empty>" : feedback;
            Log.DLazy(() => $"{GetColoredName(sender)}: Submit feedback | rating: {rating} | feedback: {text}", this);
        }

        private void OnSoundEmitted(IEventSender sender, SoundEmissionData soundData)
        {
            if (!logSoundEmitted) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Sound emitted | {DescribeSoundEmissionData(soundData)}", this);
        }

        private void OnPlayerMovement(IEventSender sender, MovementCodeEnum movementCode)
        {
            if (!logPlayerMovement) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Player movement | code: {movementCode}", this);
        }

        private void OnInteraction(IEventSender sender, InteractionEnum interactionCode)
        {
            if (!logInteraction) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Interaction | code: {interactionCode}", this);
        }

        private void OnItemUsed(IEventSender sender, SOPickable toolUsed)
        {
            if (!logItemUsed) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Item used | tool: {(toolUsed != null ? toolUsed.name : "null")}", this);
        }

        private void OnEchoMaterialUpdated(IEventSender sender, bool active)
        {
            if (!logEchoMaterialUpdated) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Echo material updated | active: {active}", this);
        }

        private void OnUITrigger(IEventSender sender, UITriggerEnum triggerCode)
        {
            if (!logUITrigger) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: UI trigger | code: {triggerCode}", this);
        }

        private void OnTimerEvent(IEventSender sender, TimerEventEnum timerEventEnum)
        {
            if (!logTimerEvent) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Timer event | code: {timerEventEnum}", this);
        }

        private void OnEnemyStateTransition(IEventSender sender, EnemyStateEnum from, EnemyStateEnum to, Transform enemyTransform)
        {
            if (!logEnemyStateTransition) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Enemy state transition {from} -> {to} | enemy: {DescribeTransform(enemyTransform)}", this);
        }

        private string GetColoredName(IEventSender sender)
        {
            string color = sender != null ? SenderColorCache.GetColor(sender) : "#FFFFFF";
            string name = sender != null ? sender.SenderName : "<null sender>";
            return $"<color={color}>[{name}]</color>";
        }

        private string DescribeEventParams(EventParams eventParams)
        {
            if (eventParams == null) return "null";

            if (eventParams is LoadingParams loadingParams)
            {
                return $"LoadingParams(isLoading: {loadingParams.IsLoading})";
            }

            if (eventParams is NarrationParams dialogParams)
            {
                DialogLine[] lines = dialogParams.NarrationContainer.DialogLines;
                int count = lines != null ? lines.Length : 0;
                return $"DialogParams(lines: {count})";
            }

            if (eventParams is HudParams hudParams)
            {
                return $"HudParams(state: {hudParams.HudState})";
            }

            if (eventParams is PopUpMessageParams warningParams)
            {
                return $"WarningParams(message: {warningParams.Message}, color: {warningParams.Color})";
            }

            if (eventParams is ToHUDStateParams toHudStateParams)
            {
                return $"ToHUDStateParams(state: {toHudStateParams.HudState})";
            }

            if (eventParams is ToNarrationParams toDialogueStateParams)
            {
                DialogLine[] lines = toDialogueStateParams.NarrationContainer.DialogLines;
                int count = lines != null ? lines.Length : 0;
                return $"ToDialogueStateParams(lines: {count})";
            }

            if (eventParams is ToLoadingStateParams toLoadingStateParams)
            {
                return $"ToLoadingStateParams(isLoading: {toLoadingStateParams.IsLoading})";
            }
            return eventParams.GetType().Name;
        }

        private string DescribeSoundEmissionData(SoundEmissionData data)
        {
            SoundClass soundClass = data.SoundClass;
            return $"pos: {data.Position}, radius: {data.Radius}, intensity: {data.Intensity}, duration: {data.Duration}, freq: {soundClass.Frequency}, env: {data.IsEnvironmental}";
        }

        private string DescribePickableData(PickableData data)
        {
            return $"pickable: {data.Name} ({data.PickableEnum}/{data.PickableType})";
        }

        private string DescribeTransform(Transform targetTransform)
        {
            if (targetTransform == null)
            {
                return "null";
            }

            return $"pos: {targetTransform.position}";
        }

        void OnValidate()
        {
            if (!activateDeactivateAll)
            {
                return;
            }

            activateDeactivateAll = false;
            bool newValue = !logAttractionInfo;

            logAttractionInfo = newValue;
            logEchoMaterialUpdated = newValue;
            logEnemyStateTransition = newValue;
            logEquippedItemChanged = newValue;
            logGameManagerStateTransition = newValue;
            logInteraction = newValue;
            logInventoryChanged = newValue;
            logItemUsed = newValue;
            logLevelAction = newValue;
            logPlayerInput = newValue;
            logPlayerMovement = newValue;
            logQuestUpdated = newValue;
            logSceneLoaderTrigger = newValue;
            logSetMaterial = newValue;
            logSetPlayerOnSpawn = newValue;
            logShowInteraction = newValue;
            logShowUI = newValue;
            logSoundEmitted = newValue;
            logSubmitFeedback = newValue;
            logSwitchLevel = newValue;
            logSwitchToGameState = newValue;
            logTimerEvent = newValue;
            logUITrigger = newValue;
        }

#endif
    }
}
