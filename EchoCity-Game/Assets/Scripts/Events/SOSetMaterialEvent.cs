using System;
using UnityEngine;

namespace EchoCity
{
    public enum EchoMaterialCodeEnum
    {
        Active,
        Inactive,
        ReApply,
        Toggle
    }
    [CreateAssetMenu(fileName = "SetMaterialEvent", menuName = "ECHO CITY/Events/Set Material")]
    public class SOSetMaterialEvent : SOSigleParamEvent<EchoMaterialCodeEnum> { }
}