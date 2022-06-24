using System;
using UnityEngine;

namespace PullOver.Gameplay.Cars
{
    [RequireComponent(typeof(BoxCollider))]
    public class Detector : MonoBehaviour
    {
        public event Action<GameObject> InRange;
        public event Action<GameObject> OutOfRange;

        private BoxCollider boxCollider;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();

            boxCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            InRange?.Invoke(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            OutOfRange?.Invoke(other.gameObject);
        }

        public void TurnOffDetection()
        {
            boxCollider.enabled = false;
        }

        public void TurnOnDetection()
        {
            boxCollider.enabled = true;
        }
    }
}
