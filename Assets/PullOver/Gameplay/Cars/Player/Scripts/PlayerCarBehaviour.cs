using Cysharp.Threading.Tasks;
using DG.Tweening;
using PullOver.Gameplay.Camera;
using PullOver.Gameplay.Cars.SO;
using PullOver.Gameplay.Level;
using PullOver.Gameplay.VFX;
using System;
using UnityEngine;
using Zenject;

namespace PullOver.Gameplay.Cars.Player
{
    [RequireComponent(typeof(CarController))]
    public class PlayerCarBehaviour : MonoBehaviour, IResetComponent
    {
        public event Action Crashe;

        [SerializeField]
        private Transform carMeshTransform;
        [Space]
        [SerializeField]
        private float movemantSpeedEEffectChangeDuration;
        [Header("Brake")]
        [SerializeField]
        private float brakeAngleX;
        [SerializeField]
        private float accelerationAngleX;
        [Space]
        [Header("Smoke")]
        [SerializeField]
        private ParticleSystem leftFrontWheelSmoke;
        [SerializeField]
        private ParticleSystem rightFrontWheelSmoke;

        private Rigidbody body;
        private Detector carDetector;
        private CarsSettings carsSettings;
        private CarController carController;
        private CarsSpawnPoint carsSpawnPoint;
        private CarsVfxManager carsVFXManager;
        private LevelBehaviour levelBehaviour;
        private Tween carMeshTransformRotation;
        private CameraController cameraController;

        private Vector3 brakeAngle;
        private Vector3 accelerationAngle;
        private Vector3 startPosition;
        private Quaternion startRotation;

        public CarController CarController
        {
            get
            {
                return carController;
            }
        }

        public bool AlreadyCrashed { get; private set; }

        [Inject]
        public void Construct(CarsSettings carsSettings)
        {
            this.carsSettings = carsSettings;
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            carController = GetComponent<CarController>();
            carDetector = GetComponentInChildren<Detector>();
            levelBehaviour = FindObjectOfType<LevelBehaviour>();
            carsVFXManager = FindObjectOfType<CarsVfxManager>();
            cameraController = FindObjectOfType<CameraController>();
            carsSpawnPoint = GetComponentInChildren<CarsSpawnPoint>();

            brakeAngle = new Vector3(brakeAngleX, 0f, 0f);
            accelerationAngle = new Vector3(accelerationAngleX, 0f, 0f);
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        private void Start()
        {
            StartMovement();
        }

        private void OnEnable()
        {
            carDetector.InRange += OnInRange;
            carDetector.OutOfRange += OnOutOfRange;
            carsSpawnPoint.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            carDetector.InRange -= OnInRange;
            carDetector.OutOfRange -= OnOutOfRange;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (AlreadyCrashed)
            {
                return;
            }

            AlreadyCrashed = true;

            Crashe?.Invoke();
            carsVFXManager.BlowUp(collision.contacts[0].point);

            carController.Stop();
            carDetector.TurnOffDetection();
            carsSpawnPoint.gameObject.SetActive(false);

            cameraController.Shake();
            leftFrontWheelSmoke.Stop();
            rightFrontWheelSmoke.Stop();
        }

        public void StartMovement()
        {
            AlreadyCrashed = false;

            leftFrontWheelSmoke.Stop();
            rightFrontWheelSmoke.Stop();

            carDetector.TurnOnDetection();
            carsSpawnPoint.gameObject.SetActive(true);
            carController.UpdateSpeedSmooth(carsSettings.PlayerMovementSpeed, carsSettings.CarAccelerationTime);
        }

        public async void SmoothStop()
        {
            carDetector.TurnOffDetection();
            carController.UpdateSpeedSmooth(0f, carsSettings.CarAccelerationTime);

            Brake();

            await UniTask.Delay(1000 * (int)carsSettings.CarAccelerationTime);

            carController.Stop();
        }

        public void ResetComponent()
        {
            body.velocity = Vector3.zero;

            carMeshTransformRotation?.Kill();
            carsSpawnPoint.ResetToDefault();

            transform.position = startPosition;
            transform.rotation = startRotation;
            transform.localRotation = Quaternion.identity;

            StartMovement();
        }

        private void OnInRange(GameObject car)
        {
            if (levelBehaviour.IsOver)
            {
                return;
            }

            CarController inTriggerCar;

            if (car.TryGetComponent(out inTriggerCar) == false)
            {
                return;
            }

            carController.UpdateSpeedSmooth(carsSettings.CarMovementSpeed,
                GetAccelerationTimeWithCorrecition(carsSettings.CarMovementSpeed));

            Brake();
        }

        private async void OnOutOfRange(GameObject car)
        {
            if (levelBehaviour.IsOver)
            {
                return;
            }

            await UniTask.Delay(500);

            carController.UpdateSpeedSmooth(carsSettings.PlayerMovementSpeed,
                GetAccelerationTimeWithCorrecition(carsSettings.PlayerMovementSpeed));

            Acceleration();
        }

        private void Brake()
        {
            carMeshTransformRotation?.Kill();

            leftFrontWheelSmoke.Play();
            rightFrontWheelSmoke.Play();

            carMeshTransformRotation = carMeshTransform.DOLocalRotate(brakeAngle, movemantSpeedEEffectChangeDuration).OnComplete(() =>
            {
                carMeshTransformRotation = carMeshTransform.DOLocalRotate(Vector3.zero, movemantSpeedEEffectChangeDuration);
            });
        }

        private void Acceleration()
        {
            carMeshTransformRotation?.Kill();

            leftFrontWheelSmoke.Play();
            rightFrontWheelSmoke.Play();

            carMeshTransformRotation = carMeshTransform.DOLocalRotate(accelerationAngle, movemantSpeedEEffectChangeDuration).OnComplete(() =>
            {
                carMeshTransformRotation = carMeshTransform.DOLocalRotate(Vector3.zero, movemantSpeedEEffectChangeDuration);
            });
        }

        private float GetAccelerationTimeWithCorrecition(float speedCarInFront)
        {
            float correction = carsSettings.CarAccelerationTime * carController.Speed / speedCarInFront;

            if (carController.Speed > speedCarInFront)
            {
                return 1 - correction;
            }

            return correction;
        }
    }
}