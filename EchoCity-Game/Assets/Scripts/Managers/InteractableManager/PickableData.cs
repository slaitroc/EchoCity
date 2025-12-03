using UnityEngine;

namespace EchoCity
{
    public struct PickableData
    {
        private PickableType pickableType;
        private string name;
        private string description;
        private int maxStackQuantity;
        private Sprite icon;
        private SOSoundSource pickUpSound;
        private SOSoundSource toolSound;
        private SOSoundSource dropSound;

        public PickableType PickableType => pickableType;
        public string Name => name;
        public string Description => description;
        public int MaxStackQuantity => maxStackQuantity;
        public Sprite Icon => icon;
        public SOSoundSource PickUpSound => pickUpSound;
        public SOSoundSource ToolSound => toolSound;
        public SOSoundSource DropSound => dropSound;

        public PickableData(SOPickable pickableSO)
        {
            pickableType = pickableSO.PickableType;
            name = pickableSO.PickableName;
            description = pickableSO.PickableDescription;
            maxStackQuantity = pickableSO.MaxStackQuantity;
            icon = pickableSO.PickableIcon;
            pickUpSound = pickableSO.PickUpSound;
            toolSound = pickableSO.ToolSound;
            dropSound = pickableSO.DropSound;
        }
    }
    public enum PickableType
    {
        Tool,
        SoundTool,
    }
}