using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "PickableSO", menuName = "ECHO CITY/Interactables/PickableSO")]
    public class SOPickableData : ScriptableObject
    {
        [SerializeField] private SOSoundSource toolSound;
        [SerializeField] private SOSoundSource pickUpSound;
        [SerializeField] private SOSoundSource dropSound;
        [SerializeField] private bool isSoundSource;
        [SerializeField] private string pickableName;
        [SerializeField] private Sprite pickableIcon;
        [TextArea(3, 10)]
        [SerializeField] private string pickableDescription;



        public SOSoundSource PickUpSound => pickUpSound;
        public SOSoundSource DropSound => dropSound;
        public SOSoundSource SoundSource => toolSound;
        public bool IsSoundSource => isSoundSource;
        public string PickableName => pickableName;
        public Sprite PickableIcon => pickableIcon;
        public string PickableDescription => pickableDescription;



        void OnValidate()
        {
            if (toolSound == null)
            {
                isSoundSource = false;
            }
            else
            {
                isSoundSource = true;
            }
        }
    }
}