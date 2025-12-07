using UnityEngine;

namespace EchoCity
{
    public abstract class SOPickable : ScriptableObject
    {
        protected virtual PickableType _pickableType { get; }
        [SerializeField] protected string pickableName;
        [TextArea(3, 10)]
        [SerializeField] protected string pickableDescription;
        [SerializeField] protected int maxStackQuantity = 1;
        [SerializeField] protected Sprite pickableIcon;
        [SerializeField] protected SOSoundSource pickUpSound;
        [SerializeField] protected SOSoundSource toolSound;
        [SerializeField] protected SOSoundSource dropSound;
        [SerializeField] protected GameObject pickablePrefab;

        public virtual string PickableName => pickableName;
        public virtual string PickableDescription => pickableDescription;
        public virtual int MaxStackQuantity => maxStackQuantity;
        public virtual Sprite PickableIcon => pickableIcon;
        public virtual SOSoundSource PickUpSound => pickUpSound;
        public virtual SOSoundSource ToolSound => toolSound;
        public virtual SOSoundSource DropSound => dropSound;
        public virtual GameObject PickablePrefab => pickablePrefab;
        public virtual PickableType PickableType => _pickableType;

    }
}