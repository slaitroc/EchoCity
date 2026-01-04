using System;
using UnityEngine;


namespace EchoCity
{
    public enum EventSenderCategoriesEnum
    {
        None,
        GameManager,
        SceneLoader,
        Interactable,
        Echolocation,
        Puzzle,
        Player,
        Enemy,
        UI,
        Emitter,
        Utility,
        Tutorial
    }
    //Name must match exactly with the SOEventBase asset names (without the "Event" suffix)
    public enum EchoCityEventsEnum
    {
        None = 0,
        AttractionInfo,
        EquippedItemChanged,
        GameStateTransition,
        InventoryChanged,
        LevelAction,
        PlayerInput,
        QuestUpdated,
        SetPlayerOnSpawn,
        ShowInteraction,
        ShowUI,
        SoundEmitted,
        SubmitFeedback,
        SwitchLevel,
        SwitchToGameState,
        ToggleMaterial,
        SetMaterial,
    }

    public abstract class SOEventBase : ScriptableObject
    {
        [SerializeField] private EchoCityEventsEnum eventType = EchoCityEventsEnum.None;
        public EchoCityEventsEnum EventType => eventType;

        void OnValidate()
        {
            // 1. Get the name of the ScriptableObject asset
            string assetName = this.name;

            if (string.IsNullOrEmpty(assetName)) return;

            // 2. Remove the word "Event" from the name (e.g: "DeathMenuEvent" -> "DeathMenu")
            // Use Replace for safety, or a more specific check
            string cleanedName = assetName.Replace("Event", "").Trim();

            // 3. Try to convert the cleaned string to an Enum value
            // The 'true' parameter ignores case sensitivity
            if (Enum.TryParse(cleanedName, true, out EchoCityEventsEnum result))
            {
                // If found, assign it
                if (eventType != result)
                {
                    eventType = result;
                    Debug.Log($"<color=green>Event automatically assigned:</color> {result} for asset {assetName}");
                }
            }
            else
            {
                // Optional: if no match is found, you can reset to None or warn the user
                // eventType = EchoCityEventsEnum.None;
                Debug.LogWarning($"<color=black>No matching Enum value found for name: {cleanedName}");
            }
        }
    }
}