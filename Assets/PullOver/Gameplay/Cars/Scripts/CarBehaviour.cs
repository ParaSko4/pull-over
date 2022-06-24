using PullOver.Gameplay.VFX;
using System;
using UnityEngine;

namespace PullOver.Gameplay.Cars
{
    [RequireComponent(typeof(CarController))]
    public class CarBehaviour : MonoBehaviour
    {
        public event Action<CarBehaviour, Vector3> CarParked;
        public event Action<CarBehaviour, Vector3> CarCrashed;

        public CarController CarController
        {
            get
            {
                return carController;
            }
        }

        public bool Sync { get; set; }

        public bool Swiped { get; private set; }

        private CarController carController;

        private bool alreadyExploded;

        private void Awake()
        {
            carController = GetComponent<CarController>();
            carController.enabled = true;
        }

        private void OnEnable()
        {
            carController.ParkingStarted += OnStartCarParked;
            carController.Parked += OnCarParked;

            alreadyExploded = false;
        }

        private void OnDisable()
        {
            Swiped = false;

            carController.ParkingStarted -= OnStartCarParked;
            carController.Parked -= OnCarParked;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (alreadyExploded)
            {
                return;
            }

            alreadyExploded = true;

            carController.Stop();
            CarCrashed?.Invoke(this, collision.contacts[0].point);
        }

        private void OnStartCarParked()
        {
            Swiped = true;
        }

        private void OnCarParked()
        {
            CarParked?.Invoke(this, transform.position);
        }
    }
}