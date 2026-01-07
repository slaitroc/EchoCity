using UnityEngine;

namespace EchoCity
{
    public class NarrationSceneInitializer : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] private SOShowUIEvent showUIEvent;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => true;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Narration_Level_Initializer };

        [SerializeField] private SODialogContainer container;

        void Start()
        {
            showUIEvent.RaiseEvent(this, ShowableUIEnum.Narration, new ToNarrationParams(container));
        }
    }

}
