using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AttackRangeDetector : MonoBehaviour
{
    public EnemyAI enemyAI;
    public SphereCollider attackCollider;

    void OnValidate()
    {
        attackCollider = GetComponent<SphereCollider>();
        attackCollider.excludeLayers = LayerMask.GetMask("Enemy", "Ignore Raycast");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        enemyAI.OnPlayerHit();

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
    }

    void OnDrawGizmos()
    {
        if (!attackCollider.enabled) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, attackCollider.radius * 2);
    }
}