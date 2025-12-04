using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "PickableDataEventSO", menuName = "ECHO CITY/Events/PickableDataEventSO")]
    public class SOPickableDataGameObjectEvent : SOEventDoubleParam<PickableData, GameObject> { }
}