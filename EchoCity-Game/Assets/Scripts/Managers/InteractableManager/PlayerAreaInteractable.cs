using UnityEngine;

namespace EchoCity
{
    public abstract class PlayerAreaInteractable : AreaInteractable
    {
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