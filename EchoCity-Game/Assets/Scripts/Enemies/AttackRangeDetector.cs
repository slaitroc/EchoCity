using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackRangeDetector : MonoBehaviour
{
    public EnemyAI enemyAI;
    public Collider attackCollider;

    void Awake()
    {
        attackCollider = GetComponent<Collider>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        enemyAI.InvokePlayerHitEvent();

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
    }
}