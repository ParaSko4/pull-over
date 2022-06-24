using DG.Tweening;
using System;
using UnityEngine;

namespace PullOver.Gameplay.Level.Values.Money
{
    public class MoneyCounter : MonoBehaviour, IResetComponent
    {
        private const int PerCarMoney = 72;

        public event Action<int> MoneyChange;

        private Tween accrualTween;
        private LevelBehaviour levelBehaviour;
        private CarsSwipeBehaviour carsSwipeBehaviour;

        private int currentSum;
        private int shouldAdded;
        private int accrualTweenValue;

        public int CurrentMoneySum
        {
            get
            {
                return currentSum;
            }
        }

        private void Awake()
        {
            levelBehaviour = FindObjectOfType<LevelBehaviour>();
            carsSwipeBehaviour = FindObjectOfType<CarsSwipeBehaviour>();
        }

        private void Update()
        {
            MoneyCalculator();
        }

        private void OnEnable()
        {
            carsSwipeBehaviour.CarSwipedSuccess += OnCarSwipedSuccess;
            carsSwipeBehaviour.CarSwipedFailure += OnCarSwipedFailure;
        }

        private void OnDisable()
        {
            carsSwipeBehaviour.CarSwipedSuccess -= OnCarSwipedSuccess;
            carsSwipeBehaviour.CarSwipedFailure -= OnCarSwipedFailure;
        }

        private void MoneyCalculator()
        {
            if (shouldAdded == 0 || levelBehaviour.IsOver)
            {
                return;
            }

            shouldAdded--;
            accrualTweenValue = currentSum;
            currentSum += PerCarMoney;

            if (accrualTween.IsActive())
            {
                accrualTween.Kill();
            }

            accrualTween = DOVirtual.Int(accrualTweenValue, currentSum, 1f, (newMoneyValue) =>
            {
                accrualTweenValue = newMoneyValue;

                MoneyChange?.Invoke(newMoneyValue);
            });
        }

        public void ResetComponent()
        {
            MoneyChange?.Invoke(0);

            currentSum = 0;
            shouldAdded = 0;

            if (accrualTween.IsActive())
            {
                accrualTween?.Kill();
            }
        }

        private void OnCarSwipedSuccess()
        {
            shouldAdded++;
        }

        private void OnCarSwipedFailure()
        {
            shouldAdded = 0;
        }
    }
}