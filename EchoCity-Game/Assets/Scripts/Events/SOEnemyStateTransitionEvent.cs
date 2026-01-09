using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "EnemyStateTransitionEvent", menuName = "ECHO CITY/Events/Enemy State Transition")]
    public class SOEnemyStateTransitionEvent : SOEventTripleParam<EnemyStateEnum, EnemyStateEnum, Transform>
    {
        public override void RaiseEvent(IEventSender sender, EnemyStateEnum fromState, EnemyStateEnum toState, Transform enemyTransform)
        {
            base.RaiseEvent(sender, fromState, toState, enemyTransform);
        }
    }
}