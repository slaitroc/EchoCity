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

        [Header("Initialization Settings")]
        [SerializeField] protected bool initInventoryOnStart = true;
        [Header("Initialization Data")]
        [SerializeField] private QuestsManager questsManager;
        [SerializeField] private SOQuest initialQuest;
        [SerializeField] private SOPickable equippedItemOnStart;

        protected PuzzleManager puzzleManager;
        protected PlayerInventory playerInventory;
        protected PlayerController playerController;

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
            EchoCitySound.StopAllVoices();
            if (triggerCode != SceneLoaderTriggerEnum.InitLevel) return;
            setPlayerOnSpawnEvent.RaiseEvent(this);
            // add respawn quest to puzzle manager
            puzzleManager = GameObject.FindGameObjectWithTag("PuzzleManager")?.GetComponent<PuzzleManager>();
            playerInventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();
            playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
            if (initInventoryOnStart && playerInventory != null)
            {
                playerInventory.Clear();
                playerController.EquipItem(0, equippedItemOnStart, null);
                Log.DLazy(() => "Player inventory initialized on level start.", this);
            }
            if (puzzleManager != null && initialQuest != null)
            {
                puzzleManager.SetQuestsManager(questsManager);
                puzzleManager.AddQuest(initialQuest);
                Log.DLazy(() => "Respawn quest added to PuzzleManager", this);
            }
            InitializeLevel();
        }
        protected virtual void InitializeLevel() { }
    }
}