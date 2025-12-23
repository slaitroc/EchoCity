
using UnityEngine;
using UnityEngine.AI;

namespace EchoCity
{
    public abstract class EnemyState : IEnemyState, IDamageDealer
    {
        protected int _animSpeedParameter = Animator.StringToHash("Speed");
        protected int _animIsAttacking = Animator.StringToHash("isAttacking");

        protected EnemyFSM _fsm;
        protected IEnemyContext _context;
        //cached references
        protected SOEnemyData _enemyData;
        protected IFSMOwner _owner;
        protected IFOV _fov;
        protected NavMeshAgent _agent;
        protected Animator _animator;
        protected PatrolArea[] _patrolAreas;
        protected AudioSource _audioSource;
        protected IHitDetector _hitDetector;
        protected PerceivedSound _perceivedSound;
        protected PerceivedSound _targetSound;
        protected IAttractionSystem _attractionSystem;
        protected IConfusionSystem _confusionSystem;
        protected SOSoundEmissionDataEvent _newAudioSphereEvent;
        protected SOSoundEmissionDataEvent _newPerceivedSoundEvent;


        public EnemyState(IEnemyContext context, EnemyFSM fsm)
        {
            _fsm = fsm;
            _context = context;

            //caching
            _enemyData = _context.EnemyData;
            _owner = _context.Owner;
            _fov = _context.FOV;
            _agent = _context.Agent;
            _animator = _context.Animator;
            _patrolAreas = _context.PatrolAreas;
            _audioSource = _context.AudioSource;
            _hitDetector = _context.HitDetector;
            _perceivedSound = _context.LastPerceivedSound;
            _targetSound = _context.TargetSound;
            _attractionSystem = _context.AttractionSystem;
            _confusionSystem = _context.ConfusionSystem;
            _newAudioSphereEvent = _context.NewAudioSphereEvent;
            _newPerceivedSoundEvent = _context.NewPerceivedSoundEvent;
        }

        public abstract EnemyStatesEnum GetEnum();
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();

        #region Animation Helper Methods
        /// <summary>
        /// Plays the StandAndExamine animation using trigger.
        /// Resets other special animation triggers and sets Speed to 0.
        /// </summary>
        protected void PlayStandAndExamineAnimation()
        {
            if (_animator == null) return;
            _animator.ResetTrigger("Trig_LostTarget");
            _animator.ResetTrigger("Trig_Confused");
            _animator.SetTrigger("Trig_StandAndExamine");
            _animator.SetFloat(_animSpeedParameter, 0f, 0.1f, Time.deltaTime);
        }

        /// <summary>
        /// Plays the LostTarget animation using trigger.
        /// Resets other special animation triggers and sets Speed to 0.
        /// </summary>
        protected void PlayLostTargetAnimation()
        {
            if (_animator == null) return;
            _animator.ResetTrigger("Trig_StandAndExamine");
            _animator.ResetTrigger("Trig_Confused");
            _animator.SetTrigger("Trig_LostTarget");
            _animator.SetFloat(_animSpeedParameter, 0f, 0.1f, Time.deltaTime);
        }

        /// <summary>
        /// Plays the Confused animation using trigger.
        /// Resets other special animation triggers and sets Speed to 0.
        /// </summary>
        protected void PlayConfusedAnimation()
        {
            if (_animator == null) return;
            _animator.ResetTrigger("Trig_StandAndExamine");
            _animator.ResetTrigger("Trig_LostTarget");
            _animator.SetTrigger("Trig_Confused");
            _animator.SetFloat(_animSpeedParameter, 0f, 0.1f, Time.deltaTime);
        }
        #endregion

        public abstract void DealDamage(IDamageable damageable);
    }
}