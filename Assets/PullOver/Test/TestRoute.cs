using UnityEngine;

[ExecuteAlways]
public class TestRoute : MonoBehaviour
{
    [SerializeField]
    private Vector3 p0;
    [SerializeField]
    private Vector3 p1;
    [SerializeField]
    private Vector3 p2;
    [SerializeField]
    private Vector3 p3;
    [Space]
    [SerializeField]
    private Transform obj;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (obj == null)
        {
            Gizmos.DrawSphere(p0, 0.3f);
            Gizmos.DrawSphere(p1, 0.3f);
            Gizmos.DrawSphere(p2, 0.3f);
            Gizmos.DrawSphere(p3, 0.3f);

            return;
        }
        else
        {
            Gizmos.DrawSphere(obj.position, 0.3f);
            Gizmos.DrawSphere(obj.position + p1, 0.3f);
            Gizmos.DrawSphere(obj.position + p2, 0.3f);
            Gizmos.DrawSphere(obj.position + p3, 0.3f);

            //p0 = obj.position;
            //p1 = obj.position + p1;
            //p2 = obj.position + p2;
            //p3 = obj.position + p3;

            Gizmos.DrawSphere(p0, 0.3f);
            Gizmos.DrawSphere(p1, 0.3f);
            Gizmos.DrawSphere(p2, 0.3f);
            Gizmos.DrawSphere(p3, 0.3f);
        }
    }
}
