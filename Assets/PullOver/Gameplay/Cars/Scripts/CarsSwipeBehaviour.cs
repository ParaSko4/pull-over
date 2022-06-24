using PullOver.Gameplay.Cars;
using PullOver.Gameplay.Cars.Enum;
using PullOver.Gameplay.Level;
using PullOver.Gameplay.Road;
using PullOver.Gameplay.Road.CrossRoad;
using PullOver.Gameplay.VFX;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PullOver.Gameplay
{
    public class CarsSwipeBehaviour : MonoBehaviour, IResetComponent
    {
        public event Action CarSwipedSuccess;
        public event Action CarSwipedFailure;

        private LinkedList<CarBehaviour> carsForSwipe = new LinkedList<CarBehaviour>();
        private Route route;
        private LevelBehaviour levelBehaviour;
        private CarsVfxManager carsVFXManager;
        private InputController inputController;
        private CrossRoadManager crossRoadManager;
        private CrossRoadBehaviour crossRoadBehaviour;

        public CarBehaviour FirstCarForSwipe
        {
            get
            {
                return carsForSwipe.ElementAtOrDefault(0);
            }
        }

        public bool StopReacting { get; set; }

        public int CarsInLineForSwipeCount
        {
            get
            {
                return carsForSwipe.Count;
            }
        }

        [Inject]
        public void Construct(Route route)
        {
            this.route = route;
        }

        private void Awake()
        {
            levelBehaviour = FindObjectOfType<LevelBehaviour>();
            carsVFXManager = FindObjectOfType<CarsVfxManager>();
            inputController = FindObjectOfType<InputController>();
            crossRoadManager = FindObjectOfType<CrossRoadManager>();
        }

        private void OnEnable()
        {
            inputController.Swiped += OnSwiped;
        }

        private void OnDisable()
        {
            inputController.Swiped -= OnSwiped;
        }

        public void ResetComponent()
        {
            carsForSwipe.Clear();
            RemoveCrossRoad();
        }

        public void AddInFront(CarBehaviour car)
        {
            carsForSwipe.AddFirst(car);
        }

        public void AddInQueue(CarBehaviour car)
        {
            carsForSwipe.AddLast(car);
        }

        public void SetCrossRoad(CrossRoadBehaviour crossRoad)
        {
            crossRoadBehaviour = crossRoad;
        }

        public void RemoveCrossRoad()
        {
            crossRoadBehaviour = null;
        }

        public void Swipe(TurningSide turningSide)
        {
            if (carsForSwipe.Count == 0 || levelBehaviour.IsOver)
            {
                return;
            }

            CarBehaviour carBehaviour = carsForSwipe.First();
            carBehaviour.CarParked += OnCarParked;
            carBehaviour.CarCrashed += OnCarCrashed;
            carsForSwipe.RemoveFirst();

            if (carBehaviour.Sync == false)
            {
                carBehaviour.CarController.UpdateSpeedSmooth(0f, 0.5f);

                return;
            }

            if (crossRoadManager != null)
            {
                if (crossRoadManager.CrossRoad)
                {
                    carBehaviour.CarController.MovePointByPoint(
                        route.GetAlongPathOnCrossRoad(carBehaviour.transform,
                            turningSide == TurningSide.Left ? crossRoadBehaviour.LeftTurnPosition : crossRoadBehaviour.RightTurnPosition),
                        route.Curve,
                        route.TravelTime);
                }

                return;
            }

            if (turningSide == TurningSide.Left)
            {
                carBehaviour.CarController.MovePointByPoint(route.GetAlongPath(carBehaviour.transform), route.Curve, route.TravelTime);
            }
            else
            {
                carBehaviour.CarController.MovePointByPoint(route.GetAlongMirrorPath(carBehaviour.transform), route.Curve, route.TravelTime);
            }
        }

        private void OnSwiped(TurningSide turningSide)
        {
            if (StopReacting)
            {
                return;
            }

            Swipe(turningSide);
        }

        private void OnCarParked(CarBehaviour carBehaviour, Vector3 parkedCarPosition)
        {
            CarSwipedSuccess?.Invoke();

            carsVFXManager.Parked(parkedCarPosition);

            carBehaviour.CarParked -= OnCarParked;
            carBehaviour.CarCrashed -= OnCarParked;
        }

        private void OnCarCrashed(CarBehaviour carBehaviour, Vector3 crashedPosition)
        {
            CarSwipedFailure?.Invoke();

            carsVFXManager.BlowUp(crashedPosition);

            carBehaviour.CarParked -= OnCarParked;
            carBehaviour.CarCrashed -= OnCarParked;
        }
    }
}
