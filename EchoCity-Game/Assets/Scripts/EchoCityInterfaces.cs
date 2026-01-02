using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace EchoCity
{
    //FSM 
    public interface IFSMOwner
    {
        //<summary> Starts a coroutine </summary>
        Coroutine StartCoroutine(IEnumerator routine);
        //<summary> Stops a coroutine </summary>
        void StopCoroutine(IEnumerator routine);
        //<summary> The owner's transform </summary>
        Transform Transform { get; }
        //<summary> The owner's game object </summary>
        GameObject GameObject { get; }
    }

    public interface IState
    {
        //<summary> Called when entering the state </summary>
        void Enter();
        //<summary> Called on every frame update while in the state </summary>
        void Update();
        //<summary> Called when exiting the state </summary>
        void Exit();
    }

    public interface IStateWithLoading : IState
    {
        //<summary> Called when entering the loading state from another state </summary>
        void EnterLoading();
        //<summary> Called when exiting the loading state to another state </summary>
        void ExitLoading();
    }

    public interface IFSM
    {
        //<summary> Initializes the FSM </summary>
        void Initialize();
        //<summary> Called on every frame update: calls Update on the current state </summary>
        void Update();
    }

    public interface IFSMWithLoading : IFSM
    {
        //<summary> Called when entering the loading state </summary>
        void EnterLoading();
        //<summary> Called when exiting the loading state </summary>
        void ExitLoading();
    }

    //GAME MANAGER
    public interface IGMContext : IEventSender
    {
        // <summary> The FSM owner </summary>
        IFSMOwner Owner { get; }
        // <summary> FSM current state </summary>
        GameStatesEnum CurrentStateEnum { get; set; }

        SOGameManagerStateTransitionEvent SwitchGameStateEvent { get; }

        // INPUT EVENTS
        SOEventVoid EnablePlayerInputEvent { get; }
        SOEventVoid DisablePlayerInputEvent { get; }
        SOEventVoid EnableUIInputEvent { get; }
        SOEventVoid DisableUIInputEvent { get; }

        // UI MANAGER EVENTS
        SOShowUIEvent ShowUIEvent { get; }

        // SCENE MANAGEMENT EVENTS
        SOEventVoid SetPlayerOnSpawnEvent { get; }
        SOSceneEnumEvent LoadLevelEvent { get; }
        SOEventVoid UnloadCurrentLevelEvent { get; }
        SOEventVoid ReloadLevelEvent { get; }


    }
    public interface IGameState : IStateWithLoading
    {
        GameStatesEnum GetEnum();
        void SwitchToTitleHandler();
        void InitLevelHandler(SceneEnum scene);
        void SwitchToPlayingHandler();
        void SwitchToPauseHandler();
        void SwitchToDeathHandler();
        void SwitchToWinHandler();
        void SwitchToNarrationHandler(DialogData data);
        void SwitchToHudHandler(HudEnum hud);
    }

    public interface IGameStatesFSM : IFSMWithLoading
    {
        IGameState CurrentState { get; }
        IGameState PreviousState { get; }
        void SwitchState(IGameState state);
        void SwitchStateUpdateOnly(IGameState state);
    }

    //ENEMY AI
    public interface IEnemyContext : IEventSender
    {
        // <summary> Enemy data Scriptable Object </summary>
        SOEnemyData EnemyData { get; }
        // <summary> The enemy who owns the FSM </summary>
        IFSMOwner Owner { get; }
        // <summary> The enemy's FSM current state </summary>
        EnemyStatesEnum CurrentStateEnum { get; set; }
        // <summary> The enemy's audio source component</summary>
        AudioSource AudioSource { get; }
        // <summary> The enemy's animator component</summary>
        Animator Animator { get; }
        // <summary> The enemy's NavMeshAgent component</summary>
        NavMeshAgent Agent { get; }
        // <summary> The enemy's Field of View system</summary>
        IFOV FOV { get; }
        // <summary> The enemy's patrol areas</summary>
        PatrolArea[] PatrolAreas { get; }
        // <summary> The enemy's currently active patrol area</summary>
        PatrolArea CurrentPatrolArea { get; set; }
        // <summary> The enemy's hit detector component</summary>
        IHitDetector HitDetector { get; }
        // <summary> The enemy's last perceived sound </summary>
        PerceivedSound LastPerceivedSound { get; }
        // <summary> The enemy's target sound </summary>
        PerceivedSound TargetSound { get; }
        // <summary> The enemy's attraction system</summary>
        IAttractionSystem AttractionSystem { get; }
        // <summary> The enemy's confusion system</summary>
        IConfusionSystem ConfusionSystem { get; }
        // <summary> Echolocation enemy's events </summary>
        SOSoundEmissionDataEvent NewAudioSphereEvent { get; }
        // <summary> enemyAttraction event </summary>
        SOIAttractionEvent EnemyAttractionEvent { get; }
    }

    public interface IEnemyState : IState, IDamageDealer //TODO
    {
        //<summary> Returns the enum associated with the state </summary>
        EnemyStatesEnum GetEnum();
    }

    //TODO
    public interface IEnemyStatesFSM : IFSM
    {
        IEnemyState CurrentState { get; }
        IEnemyState PreviousState { get; }
        void SwitchState(IEnemyState newState);
    }

    // DAMAGE   
    public interface IHitDetector
    {
        //<summary> Enables the hit detector to deal damage on hit </summary>
        void Enable();
        //<summary> Disables the hit detector to stop dealing damage on hit </summary>
        void Disable();
        //<summary> Called when a hit is detected, passing the damage dealer and damageable entities involved and applying the logic</summary>
        void HitDetected(IDamageDealer damageDealer, IDamageable damageable);
    }

    public interface IDamageDealer { void DealDamage(IDamageable damageable); }

    public interface IDamageable { void TakeDamage(float amount); }

    //ATTRAACTION/CONFUSION
    public interface IAttraction
    {
        //<summary> Current attraction level </summary>
        float CurrentAttraction { get; }
    }

    public interface IAttractionSystem : IAttraction
    {
        //<summary> Attraction computation logic </summary>
        void AttractionComputation();
        //<summary> Whether to compute attraction or not </summary>
        bool Compute { get; set; }
        //<summary> Sets the current attraction to a specific value </summary>
        void SetAttraction(float value);
        //<summary> The last perceived sound above threshold </summary>
        PerceivedSound LastPerceivedSound { get; set; }
    }

    public interface IConfusion
    {
        // <summary> Current confusion level </summary>
        float CurrentConfusion { get; }
    }

    public interface IConfusionSystem : IConfusion
    {
        // <summary> Confusion computation logic </summary>
        void ConfusionComputation();
        // <summary> Whether to compute confusion or not </summary>
        bool Compute { get; set; }
        // <summary> Sets the current confusion to a specific value </summary>
        void SetConfusion(float value);
        //<summary> The last perceived sound above threshold </summary>
        PerceivedSound LastPerceivedSound { get; set; }
    }

    //FOV
    public interface IHasFOV
    {
        IFOV FOV { get; }
        void InitializeFOV(FOVParams fovData);
        void UpdateFOV();
    }

    public interface IFOV
    {
        // <summary> FOV parameters </summary>
        FOVParams Params { get; }
        // <summary> FOV owner transform </summary>
        Transform Owner { get; }
        // <summary> Targets currently within the FOV </summary>
        TargetData[] TargetsInFOV { get; }
        // <summary> Closest target within the FOV </summary>
        TargetData ClosestTarget { get; }
        // <summary> Settable active target </summary>
        TargetData ActiveTarget { get; set; }
        // <summary> Initializes the FOV system </summary>
        void Initialize(FOVParams fovData, Transform owner);
        // <summary> Updates the FOV targets (including closest and active target) </summary>
        void UpdateTargets();
    }

    public interface ISoundPerceiver
    {
        //<summary> Handles a new perceived sound </summary>
        void PerceivedSoundHandler(IEventSender sender, SoundEmissionData ps);
        //<summary> The last perceived sound </summary>
        PerceivedSound LastPerceivedSound { get; }
    }

    public interface IEventSender
    {
        //<summary> Name of the sender </summary>
        string SenderName { get; }
        //<summary> ID of the sender </summary>
        int SenderID { get; }
        //<summary> Category of the sender </summary>
        bool IsManager { get; }
        EventSenderCategoriesEnum[] SenderCategory { get; }

    }
    //INTERACTABLE
    public interface IInteractable
    {
        //<summary> Triggers the interactable's interaction logic </summary>
        void Interact();
    }

    public interface IHasDescription
    {
        //<summary> The description of the object </summary>
        string Description { get; }
        //<summary> Whether the object has a description that can be shown via raycast </summary>
        bool HasRaycastDescription { get; }
        //<summary> Whether the object is interactable </summary>
        bool IsInteractable { get; }
    }

    public interface IPuzzleManager
    {
        IQuestsManager QuestsManager { get; }
        //<summary> Adds a quest to the quest manager </summary>
        bool CheckTags(PuzzleTagState[] tagsToCheck);
        //<summary> Sets specific puzzle tags </summary>
        void SetTags(PuzzleTagState[] tagsToSet);
        //<summary> Increments the count of a specific puzzle tag </summary>
        void IncrementTagCount(PuzzleTagEnum tag);
    }

    public interface IQuestsManager
    {
        SOQuest[] ActiveQuests { get; }
        int[] QuestProgression { get; }
    }
}