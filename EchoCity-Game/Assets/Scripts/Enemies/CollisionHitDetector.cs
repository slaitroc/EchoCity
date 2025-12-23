using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(SphereCollider))]
    public class CollisionHitDetector : MonoBehaviour, IHitDetector
    {
        [SerializeField] private SphereCollider damageCollider;
        [SerializeField] private EnemyAI enemyAI;
        public SphereCollider DamageCollider => damageCollider;
        public EnemyAI EnemyAI => enemyAI;

        void OnValidate()
        {
            damageCollider = GetComponent<SphereCollider>();
            DamageCollider.excludeLayers = LayerMask.GetMask("Enemy", "Ignore Raycast");
        }

        public void Enable()
        {
            DamageCollider.enabled = true;
        }

        public void Disable()
        {
            DamageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            HitDetected(enemyAI, other.GetComponent<IDamageable>());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
        }

        public void HitDetected(IDamageDealer damageDealer, IDamageable damageable) => enemyAI.DealDamage(damageable);

        void OnDrawGizmos()
        {
            if (!DamageCollider.enabled) return;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, DamageCollider.radius * 2);
        }


    }
}