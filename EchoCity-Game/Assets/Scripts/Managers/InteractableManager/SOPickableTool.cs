using UnityEngine;


namespace EchoCity
{
    [CreateAssetMenu(fileName = "PickableToolSO", menuName = "ECHO CITY/Interactables/PickableToolSO")]
    public class SOPickableTool : SOPickable
    {
        protected override PickableType _pickableType { get; } = PickableType.Tool;
    }
}