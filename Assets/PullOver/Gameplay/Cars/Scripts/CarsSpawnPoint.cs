using System;
using UnityEngine;

namespace PullOver.Gameplay.Cars
{
    public class CarsSpawnPoint : MonoBehaviour
    {
        public event Action MarkerInRange;

        [SerializeField]
        private Detector carDetector;
        [SerializeField]
        private Detector markerDetector;
        [SerializeField]
        private Transform spawnPosition;

        public Vector3 SpawnCarPosition
        {
            get
            {
                return spawnPosition.position;
            }
        }

        public bool SpawnPositionBusy { get; private set; }

        private void OnEnable()
        {
            carDetector.InRange += OnCarInRange;
            carDetector.OutOfRange += OnCarOutOfRange;
            markerDetector.InRange += OnMarkerInRange;
        }

        private void OnDisable()
        {
            SpawnPositionBusy = false;

            carDetector.InRange -= OnCarInRange;
            carDetector.OutOfRange -= OnCarOutOfRange;
            markerDetector.InRange -= OnMarkerInRange;
        }

        public void ResetToDefault()
        {
            SpawnPositionBusy = false;
        }

        private void OnCarInRange(GameObject car)
        {
            SpawnPositionBusy = true;
        }

        private void OnCarOutOfRange(GameObject car)
        {
            SpawnPositionBusy = false;
        }

        private void OnMarkerInRange(GameObject car)
        {
            MarkerInRange?.Invoke();
        }
    }
}