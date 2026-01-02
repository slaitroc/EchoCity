using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "IAttractionTransformBoolEvent", menuName = "ECHO CITY/Events/IAttractionTransformBoolEventEventSO")]
    public class SOIAttractionEvent : SOEventTripleParam<IAttraction, Transform, bool> { }
}