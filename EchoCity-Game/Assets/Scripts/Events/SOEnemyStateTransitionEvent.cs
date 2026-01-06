using UnityEngine;

namespace EchoCity
{
    [CreateAssetMenu(fileName = "EnemyStateTransitionEvent", menuName = "ECHO CITY/Events/Enemy State Transition")]
    public class SOEnemyStateTransitionEvent : SOEventTripleParam<EnemyStatesEnum, EnemyStatesEnum, Transform>
    {
        public override void RaiseEvent(IEventSender sender, EnemyStatesEnum fromState, EnemyStatesEnum toState, Transform enemyTransform)
        {
            base.RaiseEvent(sender, fromState, toState, enemyTransform);
        }
    }
}