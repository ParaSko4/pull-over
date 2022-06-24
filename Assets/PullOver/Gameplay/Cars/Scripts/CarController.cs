using DG.Tweening;
using PullOver.Utils;
using System;
using UnityEngine;

namespace PullOver.Gameplay.Cars
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class CarController : MonoBehaviour
    {
        public event Action ParkingStarted;
        public event Action Parked;

        private Rigidbody body;
        private Tween curveMovementTween;

        public bool CarParked { get; set; }

        public float Speed { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();

            WheelCollider[] wheels = GetComponentsInChildren<WheelCollider>();

            for (int i = 0; i < 2; i++)
            {
                wheels[i].brakeTorque = Mathf.Infinity;
            }

            foreach (Transform child in transform)
            {
                if (child.CompareTag(Tags.MassCenter))
                {
                    GetComponent<Rigidbody>().centerOfMass = child.localPosition;
                }
            }
        }

        private void Update()
        {
            Movement();
        }

        private void OnDisable()
        {
            CarParked = false;
            body.velocity = Vector3.zero;

            Stop();
        }

        public void MovePointByPoint(Vector3[] pointsPosition, AnimationCurve curve, float travelTime)
        {
            CarParked = true;

            ParkingStarted?.Invoke();
            Stop();

            curveMovementTween = DOVirtual.Float(0f, 1f, travelTime, (t) =>
            {
                float tWithCurveCorrection = curve.Evaluate(t / travelTime);

                transform.position = Bezier.GetPoint(pointsPosition[0], pointsPosition[1], pointsPosition[2], pointsPosition[3], tWithCurveCorrection);
                transform.rotation = Quaternion.LookRotation(
                    Bezier.GetFirstDerivative(pointsPosition[0], pointsPosition[1], pointsPosition[2], pointsPosition[3], tWithCurveCorrection));
            }).OnComplete(() =>
            {
                Parked?.Invoke();
            });
        }

        public void UpdateSpeedSmooth(float newSpeed, float accelerationTime)
        {
            if (curveMovementTween.IsActive())
            {
                curveMovementTween?.Kill();
            }

            curveMovementTween = DOVirtual.Float(Speed, newSpeed, accelerationTime, (newCarSpeed) =>
            {
                Speed = newCarSpeed;
            }).Play();
        }

        public void UpdateSpeed(float newSpeed)
        {
            if (curveMovementTween.IsActive())
            {
                curveMovementTween?.Kill();
            }

            Speed = newSpeed;
        }

        public void Stop()
        {
            if (curveMovementTween.IsActive())
            {
                curveMovementTween?.Kill();
            }

            Speed = 0f;
        }

        private void Movement()
        {
            if (Speed == 0f)
            {
                return;
            }

            transform.position += transform.forward * Time.deltaTime * Speed;
        }
    }
}