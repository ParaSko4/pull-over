using Cysharp.Threading.Tasks;
using PullOver.Gameplay.Cars.Player;
using PullOver.Gameplay.Cars.SO;
using PullOver.Gameplay.Road.CrossRoad;
using PullOver.Gameplay.Road.Markers;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

namespace PullOver.Gameplay.Cars
{
    public class CarsManager : MonoBehaviour, IResetComponent
    {
        private readonly Vector3 creationPosition = new Vector3(0f, -10f, 0f);

        [SerializeField]
        private int delayBeforeDeactivateCar;
        [SerializeField]
        private GameObject[] carsPrefabs;

        private Queue<CarBehaviour> busyCars = new Queue<CarBehaviour>();
        private Queue<CarBehaviour> freeCars = new Queue<CarBehaviour>();
        private Dictionary<GameObject, CarBehaviour> dicCarsBehaviours = new Dictionary<GameObject, CarBehaviour>();
        private CarsSettings carsSettings;
        private CarsSpawnPoint carsSpawnPoint;
        private CarsSwipeBehaviour carsSwipeBehaviour;
        private PlayerCarBehaviour playerCarBehaviour;
        private CancellationTokenSource cancellationTokenSource;

        private int shouldSpawned;

        public bool Stopping { get; set; }
        public int MaxActiveCars { get; set; } = 1;

        [Inject]
        public void Constuct(CarsSettings carsSettings)
        {
            this.carsSettings = carsSettings;
        }

        private void Awake()
        {
            carsSpawnPoint = FindObjectOfType<CarsSpawnPoint>();
            carsSwipeBehaviour = FindObjectOfType<CarsSwipeBehaviour>();
            playerCarBehaviour = FindObjectOfType<PlayerCarBehaviour>();

            int carsCount = FindObjectsOfType<SpawnMarker>().Length;

            CrossRoadManager crossRoadManager = FindObjectOfType<CrossRoadManager>();

            if (crossRoadManager != null)
            {
                carsCount += crossRoadManager.CrossRoadCount;
            }

            for (int i = 0; i < carsCount; i++)
            {
                CarBehaviour car = Create();
                dicCarsBehaviours[car.gameObject] = car;

                freeCars.Enqueue(car);
            }
        }

        private void Update()
        {
            SpawnCars();
        }

        private void OnEnable()
        {
            cancellationTokenSource = new CancellationTokenSource();

            carsSpawnPoint.MarkerInRange += IncreaseSpawnCarCount;
            carsSwipeBehaviour.CarSwipedSuccess += OnCarSwipedSuccess;
            carsSwipeBehaviour.CarSwipedFailure += OnCarSwipedFailure;
        }

        private void OnDisable()
        {
            carsSpawnPoint.MarkerInRange -= IncreaseSpawnCarCount;
            carsSwipeBehaviour.CarSwipedSuccess -= OnCarSwipedSuccess;
            carsSwipeBehaviour.CarSwipedFailure -= OnCarSwipedFailure;

            cancellationTokenSource.Dispose();
        }

        public void ResetComponent()
        {
            Stopping = false;
            shouldSpawned = 0;

            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();

            int busyCarsCount = busyCars.Count;

            for (int i = 0; i < busyCarsCount; i++)
            {
                ReturnCar(busyCars.Dequeue().gameObject);
            }
        }

        public CarBehaviour TakeRandomCarWithCustomRotationAndSpeed(Vector3 position, Quaternion rotation, float speed)
        {
            CarBehaviour carBehaviour = freeCars.Dequeue();
            carBehaviour.gameObject.SetActive(true);
            carBehaviour.transform.SetPositionAndRotation(position, rotation);
            carBehaviour.CarController.UpdateSpeed(speed);

            carsSwipeBehaviour.AddInFront(carBehaviour);

            return carBehaviour;
        }

        private void IncreaseSpawnCarCount()
        {
            shouldSpawned++;
        }

        private void SpawnCars()
        {
            if (Stopping 
                || shouldSpawned == 0
                || carsSpawnPoint.SpawnPositionBusy
                || carsSwipeBehaviour.CarsInLineForSwipeCount == MaxActiveCars)
            {
                return;
            }

            shouldSpawned--;

            CarBehaviour carBehaviour = freeCars.Dequeue();
            carBehaviour.gameObject.SetActive(true);
            carBehaviour.transform.SetPositionAndRotation(carsSpawnPoint.SpawnCarPosition, Quaternion.identity);
            carBehaviour.CarController.UpdateSpeed(carsSettings.CarMovementSpeed);
            carBehaviour.Sync = true;

            busyCars.Enqueue(carBehaviour);
            carsSwipeBehaviour.AddInQueue(carBehaviour);
        }

        private CarBehaviour Create()
        {
            GameObject car = Instantiate(carsPrefabs[Random.Range(0, carsPrefabs.Length)], creationPosition, Quaternion.identity);
            CarBehaviour carBehaviour = car.AddComponent<CarBehaviour>();
            carBehaviour.gameObject.SetActive(false);

            return carBehaviour;
        }

        private async void ReturnCarWithDelay()
        {
            await UniTask.Delay(delayBeforeDeactivateCar * 1000, cancellationToken: cancellationTokenSource.Token).SuppressCancellationThrow();

            if (cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            ReturnCar(busyCars.Dequeue().gameObject);
        }

        private void ReturnCar(GameObject activeCar)
        {
            CarBehaviour carBehaviour = dicCarsBehaviours[activeCar];
            carBehaviour.Sync = false;
            carBehaviour.CarController.Stop();
            carBehaviour.transform.SetPositionAndRotation(creationPosition, Quaternion.identity);
            carBehaviour.gameObject.SetActive(false);

            freeCars.Enqueue(carBehaviour);
        }

        private void OnCarSwipedFailure()
        {
            Stopping = true;

            cancellationTokenSource.Cancel();

            if (playerCarBehaviour.AlreadyCrashed == false)
            {
                playerCarBehaviour.SmoothStop();
            }
        }

        private void OnCarSwipedSuccess()
        {
            ReturnCarWithDelay();
        }
    }
}
