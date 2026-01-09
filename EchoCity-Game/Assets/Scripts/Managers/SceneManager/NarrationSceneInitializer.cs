using UnityEngine;

namespace EchoCity
{
    public class NarrationSceneInitializer : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] private SOSwitchToGameStateEvent switchToGameStateEvent;

        [Header("Observing Events")]
        [SerializeField] private SOSceneLoaderTriggerEvent sceneLoaderTriggerEvent;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => true;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Initializer };

        [Header("Initialization Data")]
        [SerializeField] private SODialogContainer container;
        [SerializeField] private SceneEnum destinationScene;

        void OnEnable()
        {
            if (sceneLoaderTriggerEvent) sceneLoaderTriggerEvent.OnEventRaised += InitializeHandler;
        }

        void OnDisable()
        {
            if (sceneLoaderTriggerEvent) sceneLoaderTriggerEvent.OnEventRaised -= InitializeHandler;
        }
        void InitializeHandler(IEventSender sender, SceneLoaderTriggerEnum triggerCode)
        {
            switchToGameStateEvent.RaiseEvent(this, GameStatesEnum.Narration, new ToNarrationParams(container, destinationScene));
        }
    }

}
