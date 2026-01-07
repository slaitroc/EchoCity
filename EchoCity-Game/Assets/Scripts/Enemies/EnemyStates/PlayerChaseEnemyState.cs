using UnityEngine;

namespace EchoCity
{
    public class PlayerChaseEnemyState : EnemyState
    {
        public PlayerChaseEnemyState(IEnemyContext context, EnemyFSM fsm) : base(context, fsm) { }

        public override EnemyStateEnum GetEnum() => EnemyStateEnum.PlayerChase;
        public override void Enter()
        {
            _context.CurrentStateEnum = EnemyStateEnum.PlayerChase;
            _hitDetector.Disable();

            _agent.speed = _enemyData.ChaseSpeed;
            _agent.isStopped = false;
            _agent.autoBraking = true;
            _agent.stoppingDistance = _enemyData.AttackRange;
            _agent.acceleration = _enemyData.ChaseAcceleration;
            _agent.angularSpeed = _enemyData.ChaseAngularSpeed;

            _attractionSystem.Compute = true;
            if (_attractionSystem.CurrentAttraction < _enemyData.At)
                _attractionSystem.SetAttraction(_enemyData.At);

            //set as target the closest visible target which triggered the state
            _fov.ActiveTarget = _fov.ClosestTarget;

            _context.HeadMark.ShowChaseMark();
        }

        public override void Update()
        {
            _animator.SetFloat(_animSpeedParameter, 1f, _enemyData.ChaseSpeedDampTime, Time.deltaTime);

            float distToTarget = Vector3.Distance(_owner.Transform.position, _fov.ActiveTarget.Transform.position);

            // Check attack range
            if (distToTarget <= _enemyData.AttackRange)
            {
                _fsm.SwitchState(_fsm.AttackState);
                return;
            }

            if (_fov.ActiveTarget.VisibilityStatus == TargetVisibilityEnum.InRangeHidden ||
                _fov.ActiveTarget.VisibilityStatus == TargetVisibilityEnum.OutOfRange)
            {
                //PlayLostTargetAnimation(); //FIX
                _fsm.SwitchState(_fsm.SoundChaseState);
                return;
            }

            _agent.SetDestination(_fov.ActiveTarget.Transform.position);
        }

        public override void Exit()
        {
            _context.HeadMark.ClearMarks();
        }

        public override void DealDamage(IDamageable damageable) { }
    }
}

