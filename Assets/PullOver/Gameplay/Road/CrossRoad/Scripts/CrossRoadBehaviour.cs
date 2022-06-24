using PullOver.Gameplay.Cars;
using System;
using UnityEngine;

namespace PullOver.Gameplay.Road.CrossRoad
{
    public class CrossRoadBehaviour : MonoBehaviour
    {
        public event Action<CrossRoadBehaviour, GameObject> CarInRange;
        public event Action<CrossRoadBehaviour, GameObject> CarOut;

        [SerializeField]
        private Transform spawnMarker;
        [Space]
        [SerializeField]
        private Transform leftTurn;
        [SerializeField]
        private Transform rightTurn;

        private CarBehaviour carBehaviour;

        public Vector3 SpawnPosition
        {
            get
            {
                return spawnMarker.position;
            }
        }

        public Vector3 LeftTurnPosition
        {
            get
            {
                return leftTurn.position;
            }
        }

        public Vector3 RightTurnPosition
        {
            get
            {
                return rightTurn.position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out carBehaviour))
            {
                if (carBehaviour.Sync == false)
                {
                    return;
                }
            }

            CarInRange?.Invoke(this, other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            CarOut?.Invoke(this, other.gameObject);
        }
    }
}
