using UnityEngine;

namespace EchoCity
{
    [System.Serializable]
    public enum HUDEnum
    {
        None = 0,
        Inventory = 1
    }

    [CreateAssetMenu(fileName = "HudEnumEventSO", menuName = "ECHO CITY/Events/SOHudEnumEvent")]
    public class SOHudEnumEvent : SOEvent<HUDEnum> { }

}
