using PullOver.Gameplay.Level.Values;
using PullOver.GUI.VFX;
using TMPro;
using UnityEngine;

namespace PullOver.GUI.Screens.Status
{
    public class TimerPresenter : MonoBehaviour
    {
        [SerializeField]
        private Transform timerImageTransform;
        [SerializeField]
        private float timerImageScale;
        [SerializeField]
        private float scaleTimerImageDuraiton;

        private Timer timer;
        private TextMeshProUGUI timerText;

        private void Awake()
        {
            timerText = GetComponentInChildren<TextMeshProUGUI>();

            timer = FindObjectOfType<Timer>();
        }

        private void OnEnable()
        {
            timer.TimerChange += OnTimerChange;
        }

        private void OnDisable()
        {
            timer.TimerChange -= OnTimerChange;
        }

        private void OnTimerChange(float time)
        {
            timerText.text = time.ToString();

            if (time < 10f && (int)time == time)
            {
                PunchEffect.Punch(timerImageTransform, timerImageScale, scaleTimerImageDuraiton);
            }
        }
    }
}