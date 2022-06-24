using UnityEngine;

namespace PullOver.Gameplay.Road.Markers
{
    [RequireComponent(typeof(BoxCollider))]
    public class SpawnMarker : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;

            transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}