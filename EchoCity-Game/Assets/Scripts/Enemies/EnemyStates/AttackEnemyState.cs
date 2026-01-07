using System.Collections;
using UnityEngine;

namespace EchoCity
{
    public class AttackEnemyState : EnemyState
    {
        private bool _attackEnded;
        private float _coolDownTimer;

        public AttackEnemyState(IEnemyContext context, EnemyFSM fsm) : base(context, fsm) { }

        public override EnemyStatesEnum GetEnum() => EnemyStatesEnum.Attack;
        public override void Enter()
        {
            _coolDownTimer = 0f;
            _attackEnded = false;

            _animator.SetBool(_animIsAttacking, true);

            _agent.isStopped = true;

            _owner.StartCoroutine(AttackRoutine());

            _context.HeadMark.ShowChaseMark();
        }
        public override void Update()
        {
            _animator.SetFloat(_animSpeedParameter, 0f, _enemyData.AttackSpeedDampTime, Time.deltaTime);

            if (_attackEnded)
            {
                Vector3 dir = _fov.ActiveTarget.Transform.position - _owner.Transform.position;
                dir.y = 0f; // Keep only horizontal direction
                Quaternion targetRot = Quaternion.LookRotation(dir);
                _owner.Transform.rotation = Quaternion.Slerp(
                    _owner.Transform.rotation,
                    targetRot,
                    _enemyData.CoolDownRotationSpeed * Time.deltaTime
                );
                _coolDownTimer += Time.deltaTime;

                if (_coolDownTimer >= _enemyData.AttackCoolDown)
                    _fsm.SwitchState(_fsm.PlayerChaseState);
            }
        }


        public override void Exit()
        {
            _animator.SetBool(_animIsAttacking, false);
            _attackEnded = false;
            _hitDetector.Disable();

            _context.HeadMark.ShowChaseMark();
        }

        IEnumerator AttackRoutine()
        {
            yield return new WaitForSeconds(_enemyData.AttackDamageDelay);
            _hitDetector.Enable();
            yield return new WaitForSeconds(_enemyData.AttackDamageWindowTime);
            _hitDetector.Disable();
            yield return new WaitForSeconds(_enemyData.AttackDuration - _enemyData.AttackDamageDelay - _enemyData.AttackDamageWindowTime);
            _animator.SetBool(_animIsAttacking, false);
            _attackEnded = true;
        }

        public override void DealDamage(IDamageable damageable) => damageable.TakeDamage(_enemyData.Damage);
    }
}