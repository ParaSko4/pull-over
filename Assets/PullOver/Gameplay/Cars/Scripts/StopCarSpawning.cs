using UnityEngine;

namespace PullOver.Gameplay.Cars
{
    [RequireComponent(typeof(BoxCollider))]
    public class StopCarSpawning : MonoBehaviour
    {
        private CarsManager carsManager;

        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;

            carsManager = FindObjectOfType<CarsManager>();

            gameObject.layer = Layouts.Marker;
        }

        private void OnTriggerEnter(Collider other)
        {
            carsManager.Stopping = true;
        }
    }
}