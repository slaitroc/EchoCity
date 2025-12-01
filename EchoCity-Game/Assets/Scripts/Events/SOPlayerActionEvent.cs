using UnityEngine;

/// <summary>
/// ScriptableObject event for notifying when the player performs an action that generates sound/noise.
/// This event is raised by InputManager (or similar system) when player actions occur.
/// </summary>
[CreateAssetMenu(fileName = "PlayerActionEventSO", menuName = "ECHO CITY/Events/PlayerActionEventSO")]
public class SOPlayerActionEvent : SOEvent<PlayerActionData> { }

