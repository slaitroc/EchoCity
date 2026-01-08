using UnityEngine;

namespace EchoCity
{
    public struct PickableData
    {
        private readonly PickablesEnum _pickableEnum;
        private readonly PickableTypeEnum _pickableType;
        private readonly string _name;
        private readonly string _description;
        private readonly int _maxStackQuantity;
        private readonly Sprite _icon;
        private readonly SOSoundSource _pickUpSound;
        private readonly SOSoundSource _toolSound;
        private readonly SOSoundSource _dropSound;

        public readonly PickablesEnum PickableEnum => _pickableEnum;
        public readonly PickableTypeEnum PickableType => _pickableType;
        public readonly string Name => _name;
        public readonly string Description => _description;
        public readonly int MaxStackQuantity => _maxStackQuantity;
        public readonly Sprite Icon => _icon;
        public readonly SOSoundSource PickUpSound => _pickUpSound;
        public readonly SOSoundSource ToolSound => _toolSound;
        public readonly SOSoundSource DropSound => _dropSound;
        public PickableData(SOPickable pickableSO)
        {
            _pickableEnum = pickableSO.PickableEnum;
            _pickableType = pickableSO.PickableType;
            _name = pickableSO.Name;
            _description = pickableSO.Description;
            _maxStackQuantity = pickableSO.MaxStackQuantity;
            _icon = pickableSO.Icon;
            _pickUpSound = pickableSO.PickUpSound;
            _toolSound = pickableSO.ToolSound;
            _dropSound = pickableSO.DropSound;
        }
    }
}