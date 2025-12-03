using UnityEngine;


namespace EchoCity
{
    [CreateAssetMenu(fileName = "PickableSoundToolSO", menuName = "ECHO CITY/Interactables/PickableSoundToolSO")]
    public class SOPickableSoundTool : SOPickable
    {
        protected override PickableType _pickableType { get; } = PickableType.SoundTool;
    }
}