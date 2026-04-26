using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.15f);
        Gizmos.DrawWireSphere(transform.position, 0.35f);
    }
}