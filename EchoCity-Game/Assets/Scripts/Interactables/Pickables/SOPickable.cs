using JetBrains.Annotations;
using System;
using UnityEngine;

namespace EchoCity
{
    public enum PickableTypeEnum
    {
        None,
        Tool,
        SoundTool,
    }

    public enum PickablesEnum
    {
        None,
        //Special Sound Tools
        Hands,
        // Sound Tools
        AirHorn,
        Carillon,
        CannedFood,
        CrowBar,
        LowGeneratorRadio,
        MetalBar,
        PowerDrill,
        Screwdriver,
        WalkieTalkie,
        Phone,
        // Tools
        Battery,
        Cable,
        FloppyDisk,
        DataCenterLaptop,
        BadgeID,
    }
    public abstract class SOPickable : ScriptableObject
    {
        [SerializeField] protected PickablesEnum _pickableEnum;
        [SerializeField] protected PickableTypeEnum _pickableType;
        [SerializeField] protected string _name;
        [TextArea(3, 10)]
        [SerializeField] protected string _description;
        [SerializeField] protected int _maxStackQuantity = 1;
        [SerializeField] protected Sprite _icon;
        [SerializeField] protected SOSoundSource _pickUpSound;
        [SerializeField] protected SOSoundSource _toolSound;
        [SerializeField] protected SOSoundSource _dropSound;
        [SerializeField] protected GameObject _prefab;

        public virtual PickablesEnum PickableEnum => _pickableEnum;
        public virtual PickableTypeEnum PickableType => _pickableType;
        public virtual string Name => _name;
        public virtual string Description => _description;
        public virtual int MaxStackQuantity => _maxStackQuantity;
        public virtual Sprite Icon => _icon;
        public virtual SOSoundSource PickUpSound => _pickUpSound;
        public virtual SOSoundSource ToolSound => _toolSound;
        public virtual SOSoundSource DropSound => _dropSound;
        public virtual GameObject Prefab => _prefab;

        void OnValidate()
        {
            string rawAssetName = this.name;
            if (string.IsNullOrEmpty(rawAssetName))
            {
                return;
            }

            string assetName = rawAssetName.Split('_')[0].Trim();

            if (!string.IsNullOrEmpty(assetName))
            {
                if (Enum.TryParse(assetName, true, out PickablesEnum result))
                {
                    if (_pickableEnum != result)
                    {
                        _pickableEnum = result;
                        Debug.Log($"<color=green>Pickable automatically assigned:</color> {result} for asset {rawAssetName}");
                    }

                    if (_name != result.ToString())
                    {
                        _name = result.ToString();
                    }
                }
                else
                {
                    Debug.LogWarning($"<color=black>No matching Enum value found for name: {assetName} (from asset {rawAssetName})");
                }
            }

            // Assign PickableType based on the type
            if (this is SOPickableTool)
            {
                _pickableType = PickableTypeEnum.Tool;
            }
            else if (this is SOPickableSoundTool)
            {
                _pickableType = PickableTypeEnum.SoundTool;
            }
        }
    }
}