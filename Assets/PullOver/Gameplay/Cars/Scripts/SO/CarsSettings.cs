using UnityEngine;

namespace PullOver.Gameplay.Cars.SO
{
    [CreateAssetMenu(fileName = "CarsSettings", menuName = "ScriptableObjects/CarsSettings")]
    public class CarsSettings : ScriptableObject
    {
        public float CarMovementSpeed
        {
            get
            {
                return carMovementSpeed;
            }
        }

        public float CarAccelerationTime
        {
            get
            {
                return carAccelerationTime;
            }
        }

        public float PlayerMovementSpeed
        {
            get
            {
                return playerMovementSpeed;
            }
        }

        [Header("Cars settings")]
        [SerializeField]
        private float carMovementSpeed;
        [SerializeField]
        private float carAccelerationTime;
        [Space]
        [Header("Player Car")]
        [SerializeField]
        private float playerMovementSpeed;
    }
}
