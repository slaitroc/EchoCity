using UnityEngine;

namespace EchoCity
{
    public abstract class PlayerAreaInteractable : AreaInteractable
    {
        protected override string _TYPE_LOG_TAG => "PLAYER";

        public override void OnEnteringRangeArea(Collider other)
        {
            if (other.CompareTag("Player")) _playerInRange = other;
        }

        public override void OnExitingRangeArea(Collider other)
        {
            if (other.CompareTag("Player")) _playerInRange = null;
        }
    }
}