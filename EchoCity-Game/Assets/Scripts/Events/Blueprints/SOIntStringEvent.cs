using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "IntStringEventSO", menuName = "ECHO CITY/Events/IntStringEventSO")]
    public class SOIntStringEvent : SOEvent<(int, string)> { }
}
