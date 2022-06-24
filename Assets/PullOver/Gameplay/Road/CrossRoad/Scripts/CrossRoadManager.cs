using PullOver.Gameplay.Cars;
using PullOver.Gameplay.Cars.Player;
using PullOver.Gameplay.Cars.SO;
using UnityEngine;
using Zenject;

namespace PullOver.Gameplay.Road.CrossRoad
{
    public class CrossRoadManager : MonoBehaviour, IResetComponent
    {
        private readonly Quaternion leftSpawnPosition = new Quaternion(0f, 90f, 0f, 0f);
        private readonly Quaternion rightSpawnPosition = new Quaternion(0f, 180f, 0f, 0f);

        private CarBehaviour spawnedCar;
        private CarsSettings carsSettings;
        private CarsManager levelCarsController;
        private CrossRoadBehaviour[] crossRoadBehaviours;
        private CrossRoadBehaviour currentCrossRoad;

        public CrossRoadBehaviour CrossRoad
        {
            get
            {
                return currentCrossRoad;
            }
        }

        public int CrossRoadCount
        {
            get
            {
                return crossRoadBehaviours.Length;
            }
        }

        [Inject]
        public void Constuct(CarsSettings carsSettings)
        {
            this.carsSettings = carsSettings;
        }

        private void Awake()
        {
            levelCarsController = FindObjectOfType<CarsManager>();
            crossRoadBehaviours = FindObjectsOfType<CrossRoadBehaviour>();
        }

        private void OnEnable()
        {
            for (int i = 0; i < crossRoadBehaviours.Length; i++)
            {
                crossRoadBehaviours[i].CarInRange += OnCarInRange;
                crossRoadBehaviours[i].CarOut += OnCarOut;
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < crossRoadBehaviours.Length; i++)
            {
                crossRoadBehaviours[i].CarInRange -= OnCarInRange;
                crossRoadBehaviours[i].CarOut -= OnCarOut;
            }
        }

        private void OnCarInRange(CrossRoadBehaviour crossRoad, GameObject car)
        {
            if (currentCrossRoad == crossRoad)
            {
                return;
            }

            currentCrossRoad = crossRoad;

            spawnedCar = levelCarsController.TakeRandomCarWithCustomRotationAndSpeed(
                GetCarPosition(car.transform.position.z),
                GetCarRotation(),
                carsSettings.PlayerMovementSpeed);

            PlayerCarBehaviour player;

            if (car.TryGetComponent(out player))
            {
                return;
            }

            spawnedCar.CarController.UpdateSpeedSmooth(0f, 2f);
        }

        private void OnCarOut(CrossRoadBehaviour crossRoad, GameObject car)
        {
            PlayerCarBehaviour player;

            if (car.TryGetComponent(out player))
            {
                currentCrossRoad = null;
            }
        }

        public void ResetComponent()
        {
            currentCrossRoad = null;
        }

        private Quaternion GetCarRotation()
        {
            if (currentCrossRoad.transform.position.x > currentCrossRoad.SpawnPosition.x)
            {
                return leftSpawnPosition;
            }

            return rightSpawnPosition;
        }

        private Vector3 GetCarPosition(float z)
        {
            float x = transform.position.z - z;
            return new Vector3(transform.position.x - x, currentCrossRoad.SpawnPosition.y, currentCrossRoad.SpawnPosition.z);
        }
    }
}