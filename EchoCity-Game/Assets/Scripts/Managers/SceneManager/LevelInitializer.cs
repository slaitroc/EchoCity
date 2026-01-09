using UnityEngine;

namespace EchoCity
{
    public class LevelInitializer : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] private SOSetPlayerOnSpawnEvent setPlayerOnSpawnEvent;

        [Header("Observing Events")]
        [SerializeField] private SOSceneLoaderTriggerEvent sceneLoaderTriggerEvent;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => true;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Initializer };

        [Header("Initialization Data")]
        [SerializeField] private QuestsManager questsManager;
        [SerializeField] private SOQuest initialTutorialQuest;

        protected PuzzleManager puzzleManager;

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
            if (triggerCode != SceneLoaderTriggerEnum.InitLevel) return;
            setPlayerOnSpawnEvent.RaiseEvent(this);
            // add respawn quest to puzzle manager
            puzzleManager = GameObject.FindGameObjectWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
            if (puzzleManager != null && initialTutorialQuest != null)
            {
                puzzleManager.SetQuestsManager(questsManager);
                puzzleManager.AddQuest(initialTutorialQuest);
                Log.DLazy(() => "Respawn quest added to PuzzleManager", this);
            }
            InitializeLevel();
        }
        protected virtual void InitializeLevel() { }
    }
}