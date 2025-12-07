using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public enum HudEnum
    {
        None = 0,
        Inventory = 1
    }

    [CreateAssetMenu(fileName = "HudEnumEventSO", menuName = "ECHO CITY/Events/SOHudEnumEvent")]
    public class SOHudEnumEvent : SOEvent<HudEnum> { }

}
