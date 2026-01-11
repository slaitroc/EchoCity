using UnityEngine;

namespace EchoCity
{
    public class LevelInitializer : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] private SOSetPlayerOnSpawnEvent setPlayerOnSpawnEvent;
        [SerializeField] private SOSetMaterialEvent setMaterialEvent;


        [Header("Observing Events")]
        [SerializeField] private SOSceneLoaderTriggerEvent sceneLoaderTriggerEvent;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => true;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Initializer };

        [Header("Initialization Settings")]
        [SerializeField] protected bool initInventoryOnStart = true;
        [SerializeField] protected bool setMaterialsActiveOnStart = true;
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
            Debug.Assert(puzzleManager != null, "PuzzleManager not found in the scene");
            Debug.Assert(playerInventory != null, "PlayerInventory not found in the scene");
            Debug.Assert(playerController != null, "PlayerController not found in the scene");
            if (initInventoryOnStart && playerInventory != null)
            {
                playerInventory.Clear();
                playerController.EquipItem(0, equippedItemOnStart, null);
                Log.DLazy(() => "Player inventory initialized on level start.", this);
            }
            if (puzzleManager != null)
            {
                puzzleManager.SetQuestsManager(questsManager);
                if (initialQuest != null)
                    puzzleManager.AddQuest(initialQuest);
                Log.DLazy(() => "PuzzleManager initialized on level start.", this);
            }
            if (setMaterialEvent != null)
            {
                if (setMaterialsActiveOnStart)
                    setMaterialEvent.RaiseEvent(this, EchoMaterialCodeEnum.Active);
                else
                    setMaterialEvent.RaiseEvent(this, EchoMaterialCodeEnum.Inactive);
            }
            InitializeLevel();
        }
        protected virtual void InitializeLevel() { }
    }
}