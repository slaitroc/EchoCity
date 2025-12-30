using UnityEngine;
using UnityEngine.AI;

namespace EchoCity
{
    public class PatrolEnemyState : EnemyState
    {
        private Transform[] _waypoints;
        private PatrolArea _tempCurrentPatrolArea;
        private int _currentWaypointIndex = 0;
        private bool _isWaitingAtWaypoint = false;
        private float _waitTimer = 0f;

        public PatrolEnemyState(IEnemyContext context, EnemyFSM fsm) : base(context, fsm) { }

        public override EnemyStatesEnum GetEnum() => EnemyStatesEnum.Patrol;
        public override void Enter()
        {
            _isWaitingAtWaypoint = false;
            _waitTimer = 0f;

            _context.CurrentStateEnum = EnemyStatesEnum.Patrol;
            _hitDetector.Disable();

            _currentWaypointIndex = EchoCityUtils.SelectClosestWaypoint(_patrolAreas, _owner.Transform, out _tempCurrentPatrolArea);
            _context.CurrentPatrolArea = _tempCurrentPatrolArea;
            _waypoints = _tempCurrentPatrolArea.Waypoints;

            Debug.Assert(_waypoints != null && _waypoints.Length > 0, "PatrolEnemyState: Waypoints array is null or empty.");


            _agent.isStopped = false;
            _agent.stoppingDistance = _enemyData.WaypointArrivalThreshold;
            _agent.autoBraking = true;
            _agent.acceleration = _enemyData.PatrolAcceleration;
            _agent.angularSpeed = _enemyData.PatrolAngularSpeed;
            _agent.speed = _enemyData.PatrolSpeed * _enemyData.ChaseSpeed;

            _attractionSystem.Compute = true;

            GotoNextWaypoint();

            //send event to player controller to update attraction HUD
            _enemyAttractionEvent?.RaiseEvent(_context, _attractionSystem, _owner.Transform, false);
        }
        public override void Update()
        {
            // //DEBUG
            // _agent.acceleration = _enemyData.PatrolAcceleration;
            // _agent.angularSpeed = _enemyData.PatrolAngularSpeed;
            // _agent.speed = _enemyData.PatrolSpeed * _enemyData.ChaseSpeed;

            if (_fov.ClosestTarget.Transform != null && _fov.ClosestTarget.VisibilityStatus == TargetVisibilityEnum.VisibleInFOV)
            {
                if (_fov.ClosestTarget.Velocity >= _enemyData.DetectionVelocity)
                {
                    _fsm.SwitchState(_fsm.PlayerChaseState);
                    return;
                }
            }

            if (_attractionSystem.CurrentAttraction >= _enemyData.At)
            {
                _fsm.SwitchState(_fsm.SoundChaseState);
                return;
            }


            float targetSpeed = _isWaitingAtWaypoint ? 0f : _enemyData.PatrolSpeed;
            _animator.SetFloat(_animSpeedParameter, targetSpeed, _enemyData.PatrolSpeedDampTime, Time.deltaTime);

            if (_isWaitingAtWaypoint)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    _isWaitingAtWaypoint = false;
                    GotoNextWaypoint();
                }
                return;
            }

            if (!_agent.pathPending && _agent.remainingDistance <= _enemyData.WaypointArrivalThreshold)
            {
                StartWaitAtWaypoint();
            }
        }

        public override void Exit()
        {
            _enemyAttractionEvent?.RaiseEvent(_context, _attractionSystem, _owner.Transform, true);
        }
        public override void DealDamage(IDamageable damageable) { }

        private void StartWaitAtWaypoint()
        {
            _isWaitingAtWaypoint = true;
            _waitTimer = _context.EnemyData.WaypointPauseDuration;

            _agent.isStopped = true;
            _agent.ResetPath();
        }

        private void GotoNextWaypoint()
        {
            _isWaitingAtWaypoint = false;
            _agent.isStopped = false;
            _agent.SetDestination(_waypoints[_currentWaypointIndex].position);
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        }



    }
}