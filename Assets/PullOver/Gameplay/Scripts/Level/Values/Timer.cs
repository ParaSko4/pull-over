using System;
using UniRx;
using UnityEngine;

namespace PullOver.Gameplay.Level.Values
{
    public class Timer : MonoBehaviour, IResetComponent
    {
        public event Action<float> TimerChange;
        public event Action TimeOver;

        [SerializeField]
        private float timeToPass;
        [SerializeField]
        private float minTimeWhenShowMilliseconds;

        private LevelBehaviour levelBehaviour;
        private IDisposable timerObservable;

        private float currentTime;

        public void Awake()
        {
            levelBehaviour = FindObjectOfType<LevelBehaviour>();
        }

        private void Start()
        {
            ChangeTime(timeToPass);
        }

        private void OnEnable()
        {
            levelBehaviour.Win += OnLevelComplete;
            levelBehaviour.Lose += OnLevelComplete;
        }

        private void OnDisable()
        {
            levelBehaviour.Win -= OnLevelComplete;
            levelBehaviour.Lose -= OnLevelComplete;

            timerObservable?.Dispose();
        }

        public void StartTimer()
        {
            timerObservable = Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(x =>
            {
                float currentTime = timeToPass - (float)x / 10;

                if (currentTime <= 0f)
                {
                    ChangeTime(0f);

                    TimeOver?.Invoke();
                    StopTimer();
                }

                ChangeTime(minTimeWhenShowMilliseconds >= currentTime ? currentTime : Mathf.Floor(currentTime));
            });
        }

        public void StopTimer()
        {
            timerObservable?.Dispose();
        }

        public void ResetComponent()
        {
            StopTimer();
            StartTimer();
        }

        private void ChangeTime(float time)
        {
            currentTime = time;

            TimerChange?.Invoke(time);
        }

        private void OnLevelComplete()
        {
            StopTimer();
        }
    }
}