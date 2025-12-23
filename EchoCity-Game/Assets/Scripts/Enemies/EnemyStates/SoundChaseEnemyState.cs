using UnityEngine;

namespace EchoCity
{
    public class SoundChaseEnemyState : EnemyState
    {
        private bool _isCheckingSound = false;
        private float _waitTimer = 0f;
        public SoundChaseEnemyState(IEnemyContext context, EnemyFSM fsm) : base(context, fsm) { }
        public override EnemyStatesEnum GetEnum() => EnemyStatesEnum.SoundChase;
        public override void Enter()
        {
            _isCheckingSound = false;
            _waitTimer = 0f;

            _context.CurrentStateEnum = EnemyStatesEnum.SoundChase;
            _hitDetector.Disable();

            _agent.isStopped = false;
            _agent.stoppingDistance = _enemyData.ChaseSoundArrivalThreshold;
            _agent.autoBraking = true;
            _agent.acceleration = _enemyData.ChaseAcceleration;
            _agent.angularSpeed = _enemyData.ChaseAngularSpeed;
            _agent.speed = _enemyData.ChaseSpeed;

            _agent.SetDestination(_attractionSystem.LastPerceivedSound.Position);

            _attractionSystem.Compute = true;

            //set as target the sound which triggered the state
            _targetSound.UpdatePerceivedSound(_attractionSystem.LastPerceivedSound);
        }

        public override void Update()
        {

            if (_fov.ClosestTarget.VisibilityStatus == TargetVisibilityEnum.VisibleInFOV)
            {
                var dist = Vector3.Distance(_fov.ClosestTarget.Transform.position, _owner.Transform.position);
                if (dist <= _enemyData.AttackRange)
                {
                    _fov.ActiveTarget = _fov.ClosestTarget;
                    _fsm.SwitchState(_fsm.AttackState);
                    return;
                }
                if (_fov.ClosestTarget.Velocity >= _enemyData.DetectionVelocity)
                {
                    _fov.ActiveTarget = _fov.ClosestTarget;
                    _fsm.SwitchState(_fsm.PlayerChaseState);
                    return;
                }
            }

            if (_attractionSystem.CurrentAttraction >= _enemyData.At && _attractionSystem.LastPerceivedSound.Position != _targetSound.Position)
            {
                _targetSound.UpdatePerceivedSound(_attractionSystem.LastPerceivedSound);
                _isCheckingSound = false;
                _agent.isStopped = false;
                _agent.SetDestination(_targetSound.Position);
            }

            if (!_isCheckingSound)
            {
                _animator.SetFloat(_animSpeedParameter, 1f, _enemyData.ChaseSpeedDampTime, Time.deltaTime);

                if (!_agent.pathPending && _agent.remainingDistance <= _enemyData.ChaseSoundArrivalThreshold)
                    StartCheckingSound();
            }
            else
            {
                _animator.SetFloat(_animSpeedParameter, 0f, _enemyData.ChaseSpeedDampTime, Time.deltaTime);
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    _fsm.SwitchState(_fsm.PatrolState);
                    return;
                }
            }
        }
        public override void Exit() { }
        public override void DealDamage(IDamageable damageable) { }
        private void StartCheckingSound()
        {
            _isCheckingSound = true;
            _waitTimer = _context.EnemyData.CheckSoundPauseDuration;
            _agent.isStopped = true;
            _agent.ResetPath();
        }

    }
}