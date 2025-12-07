using Unity.VisualScripting;
using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "PlayerActionEventSO", menuName = "ECHO CITY/Events/PlayerActionEventSO")]
    public class SOPlayerActionEvent : SOEvent<PlayerActionData> { }
}