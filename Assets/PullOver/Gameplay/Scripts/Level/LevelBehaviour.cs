using PullOver.Gameplay.Cars.Player;
using PullOver.Gameplay.Level.Values;
using System;
using UnityEngine;

namespace PullOver.Gameplay.Level
{
    public class LevelBehaviour : MonoBehaviour, IResetComponent
    {
        public event Action Win;
        public event Action Lose;

        private Timer timer;
        private LevelCompletion levelCompletion;
        private CarsSwipeBehaviour carsSwipeBehaviour;
        private PlayerCarBehaviour playerCarBehaviour;

        public bool IsOver { get; private set; }

        private void Awake()
        {
            Application.targetFrameRate = 60;

            timer = FindObjectOfType<Timer>();
            levelCompletion = FindObjectOfType<LevelCompletion>();
            carsSwipeBehaviour = FindObjectOfType<CarsSwipeBehaviour>();
            playerCarBehaviour = FindObjectOfType<PlayerCarBehaviour>();
        }

        private void OnEnable()
        {
            timer.TimeOver += OnLose;
            playerCarBehaviour.Crashe += OnLose;
            levelCompletion.PlayerInZone += OnWin;
            carsSwipeBehaviour.CarSwipedFailure += OnLose;
        }

        private void OnDisable()
        {
            timer.TimeOver -= OnLose;
            playerCarBehaviour.Crashe -= OnLose;
            levelCompletion.PlayerInZone -= OnWin;
            carsSwipeBehaviour.CarSwipedFailure -= OnLose;
        }

        public void ResetComponent()
        {
            IsOver = false;
        }

        private void OnWin()
        {
            if (IsOver)
            {
                return;
            }

            IsOver = true;

            Win?.Invoke();
        }

        private void OnLose()
        {
            if (IsOver)
            {
                return;
            }

            IsOver = true;

            Lose?.Invoke();
        }
    }
}