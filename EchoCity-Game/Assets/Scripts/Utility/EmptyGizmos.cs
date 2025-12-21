using UnityEngine;

public class EmptyGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        //forward direction
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);

        
    }
}