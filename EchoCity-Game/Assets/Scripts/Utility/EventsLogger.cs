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
        [SerializeField] private SOGameManagerStateTransitionEvent gameManagerStateTransitionEvent;
        [SerializeField] private bool logGameManagerStateTransition = true;

        [SerializeField] private SOEquippedItemChanged equippedItemChangedEvent;
        [SerializeField] private bool logEquippedItemChanged = true;

        [SerializeField] private SOSetPlayerOnSpawnEvent setPlayerOnSpawnEvent;
        [SerializeField] private bool logSetPlayerOnSpawn = true;

        [SerializeField] private SOQuestUpdatedEvent questUpdatedEvent;
        [SerializeField] private bool logQuestUpdated = true;

        [SerializeField] private SOShowInteractionEvent showInteractionEvent;
        [SerializeField] private bool logShowInteraction = true;

        [SerializeField] private SOPlayerInputEvent playerInputEvent;
        [SerializeField] private bool logPlayerInput = true;

        [SerializeField] private SOLevelActionEvent levelActionEvent;
        [SerializeField] private bool logLevelAction = true;

        [SerializeField] private SOShowUIEvent showUIEvent;
        [SerializeField] private bool logShowUI = true;

        [SerializeField] private SOInventoryChangedEvent inventoryChangedEvent;
        [SerializeField] private bool logInventoryChanged = true;

        [SerializeField] private SOAttractionInfoEvent attractionInfoEvent;
        [SerializeField] private bool logAttractionInfo = true;

        [SerializeField] private SOSetMaterialEvent setMaterialEvent;
        [SerializeField] private bool logSetMaterial = true;

        [SerializeField] private SOSwitchToGameStateEvent switchToGameStateEvent;
        [SerializeField] private bool logSwitchToGameState = true;

        [SerializeField] private SOSwitchLevelEvent switchLevelEvent;
        [SerializeField] private bool logSwitchLevel = true;

        [SerializeField] private SOSubmitFeedbackEvent submitFeedbackEvent;
        [SerializeField] private bool logSubmitFeedback = true;

        [SerializeField] private SOSoundEmittedEvent soundEmittedEvent;
        [SerializeField] private bool logSoundEmitted = true;

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
            if (gameManagerStateTransitionEvent != null) gameManagerStateTransitionEvent.OnEventRaised += OnGameManagerStateTransition;
            if (equippedItemChangedEvent != null) equippedItemChangedEvent.OnEventRaised += OnEquippedItemChanged;
            if (setPlayerOnSpawnEvent != null) setPlayerOnSpawnEvent.OnEventRaised += OnSetPlayerOnSpawn;
            if (questUpdatedEvent != null) questUpdatedEvent.OnEventRaised += OnQuestUpdated;
            if (showInteractionEvent != null) showInteractionEvent.OnEventRaised += OnShowInteraction;
            if (playerInputEvent != null) playerInputEvent.OnEventRaised += OnPlayerInput;
            if (levelActionEvent != null) levelActionEvent.OnEventRaised += OnLevelAction;
            if (showUIEvent != null) showUIEvent.OnEventRaised += OnShowUI;
            if (inventoryChangedEvent != null) inventoryChangedEvent.OnEventRaised += OnInventoryChanged;
            if (attractionInfoEvent != null) attractionInfoEvent.OnEventRaised += OnAttractionInfo;
            if (setMaterialEvent != null) setMaterialEvent.OnEventRaised += OnSetMaterial;
            if (switchToGameStateEvent != null) switchToGameStateEvent.OnEventRaised += OnSwitchToGameState;
            if (switchLevelEvent != null) switchLevelEvent.OnEventRaised += OnSwitchLevel;
            if (submitFeedbackEvent != null) submitFeedbackEvent.OnEventRaised += OnSubmitFeedback;
            if (soundEmittedEvent != null) soundEmittedEvent.OnEventRaised += OnSoundEmitted;
        }

        private void UnregisterEvents()
        {
            if (gameManagerStateTransitionEvent != null) gameManagerStateTransitionEvent.OnEventRaised -= OnGameManagerStateTransition;
            if (equippedItemChangedEvent != null) equippedItemChangedEvent.OnEventRaised -= OnEquippedItemChanged;
            if (setPlayerOnSpawnEvent != null) setPlayerOnSpawnEvent.OnEventRaised -= OnSetPlayerOnSpawn;
            if (questUpdatedEvent != null) questUpdatedEvent.OnEventRaised -= OnQuestUpdated;
            if (showInteractionEvent != null) showInteractionEvent.OnEventRaised -= OnShowInteraction;
            if (playerInputEvent != null) playerInputEvent.OnEventRaised -= OnPlayerInput;
            if (levelActionEvent != null) levelActionEvent.OnEventRaised -= OnLevelAction;
            if (showUIEvent != null) showUIEvent.OnEventRaised -= OnShowUI;
            if (inventoryChangedEvent != null) inventoryChangedEvent.OnEventRaised -= OnInventoryChanged;
            if (attractionInfoEvent != null) attractionInfoEvent.OnEventRaised -= OnAttractionInfo;
            if (setMaterialEvent != null) setMaterialEvent.OnEventRaised -= OnSetMaterial;
            if (switchToGameStateEvent != null) switchToGameStateEvent.OnEventRaised -= OnSwitchToGameState;
            if (switchLevelEvent != null) switchLevelEvent.OnEventRaised -= OnSwitchLevel;
            if (submitFeedbackEvent != null) submitFeedbackEvent.OnEventRaised -= OnSubmitFeedback;
            if (soundEmittedEvent != null) soundEmittedEvent.OnEventRaised -= OnSoundEmitted;
        }

        private void OnGameManagerStateTransition(IEventSender sender, GameStatesEnum from, GameStatesEnum to)
        {
            if (!logGameManagerStateTransition) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: GameManager state transition {from} -> {to}", this);
        }

        private void OnEquippedItemChanged(IEventSender sender)
        {
            if (!logEquippedItemChanged) return;
            Log.DLazy(() => $"{GetColoredName(sender)}: Equipped item changed", this);
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

            if (eventParams is DialogParams dialogParams)
            {
                DialogLines[] lines = dialogParams.DialogData.DialogLines;
                int count = lines != null ? lines.Length : 0;
                return $"DialogParams(lines: {count})";
            }

            if (eventParams is HudParams hudParams)
            {
                return $"HudParams(state: {hudParams.HudState})";
            }

            if (eventParams is WarningParams warningParams)
            {
                return $"WarningParams(message: {warningParams.Message}, color: {warningParams.Color})";
            }

            if (eventParams is ToHUDStateParams toHudStateParams)
            {
                return $"ToHUDStateParams(state: {toHudStateParams.HudState})";
            }

            if (eventParams is ToDialogueStateParams toDialogueStateParams)
            {
                DialogLines[] lines = toDialogueStateParams.DialogData.DialogLines;
                int count = lines != null ? lines.Length : 0;
                return $"ToDialogueStateParams(lines: {count})";
            }

            if (eventParams is ToLoadingStateParams toLoadingStateParams)
            {
                return $"ToLoadingStateParams(isLoading: {toLoadingStateParams.IsLoading})";
            }

            if (eventParams is ToLevelParams)
            {
                return "ToLevelParams";
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
            bool newValue = !logGameManagerStateTransition;

            logGameManagerStateTransition = newValue;
            logEquippedItemChanged = newValue;
            logSetPlayerOnSpawn = newValue;
            logQuestUpdated = newValue;
            logShowInteraction = newValue;
            logPlayerInput = newValue;
            logLevelAction = newValue;
            logShowUI = newValue;
            logInventoryChanged = newValue;
            logAttractionInfo = newValue;
            logSetMaterial = newValue;
            logSwitchToGameState = newValue;
            logSwitchLevel = newValue;
            logSubmitFeedback = newValue;
            logSoundEmitted = newValue;
        }

#endif
    }
}
