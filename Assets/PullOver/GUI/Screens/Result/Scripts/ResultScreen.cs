using DG.Tweening;
using PullOver.Gameplay.Level;
using PullOver.Gameplay.Level.Values.Money;
using PullOver.GUI.VFX;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PullOver.GUI.Screens.Video
{
    public class ResultScreen : BaseScreen
    {
        [SerializeField]
        private TextMeshProUGUI totalMoneySum;
        [SerializeField]
        private Image levelResultTextImage;
        [SerializeField]
        private Transform moneyImageTransform;
        [Space]
        [SerializeField]
        private Sprite levelWinTextImage;
        [SerializeField]
        private Sprite levelLoseTextImage;
        [Space]
        [SerializeField]
        private float moneyCounterDuration;
        [SerializeField]
        private float moneyCounterImageScale;
        [SerializeField]
        private float scaleMoneyImageDuraiton;
        [SerializeField]
        private int moneyBounceStep;

        private CompositeDisposable disposables = new CompositeDisposable();
        private Button restartButton;
        private MoneyCounter moneyCounter;
        private Tween moneyCounterTextTween;
        private LevelRestarter levelRestarter;
        private LevelBehaviour levelBehaviour;

        protected override void OnAwake()
        {
            restartButton = GetComponentInChildren<Button>();

            moneyCounter = FindObjectOfType<MoneyCounter>();
            levelBehaviour = FindObjectOfType<LevelBehaviour>();
            levelRestarter = FindObjectOfType<LevelRestarter>();

            restartButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (levelBehaviour.IsOver == false)
                {
                    return;
                }

                levelRestarter.ResetComponent();

                Hide();
            }).AddTo(disposables);
        }

        private void Start()
        {
            HideInstantly();
        }

        private void OnEnable()
        {
            levelBehaviour.Win += OnWin;
            levelBehaviour.Lose += OnLose;
        }

        private void OnDisable()
        {
            levelBehaviour.Win -= OnWin;
            levelBehaviour.Lose -= OnLose;
        }

        private void OnDestroy()
        {
            disposables?.Dispose();
        }

        private async void Show()
        {
            restartButton.gameObject.SetActive(true);
            moneyImageTransform.transform.localScale = Vector3.one;

            await Fade.Show();

            GetResult();
        }

        private async void Hide()
        {
            StopCountingMoney();

            await Fade.Hide();

            totalMoneySum.text = 0.ToString();
            restartButton.gameObject.SetActive(false);
        }

        private void HideInstantly()
        {
            Fade.HideInstantly();
            totalMoneySum.text = 0.ToString();

            restartButton.gameObject.SetActive(false);
        }

        private void OnLose()
        {
            levelResultTextImage.sprite = levelLoseTextImage;
            Show();
        }

        private void OnWin()
        {
            levelResultTextImage.sprite = levelWinTextImage;
            Show();
        }

        private void GetResult()
        {
            int count = 0;

            moneyCounterTextTween = DOVirtual.Int(0, moneyCounter.CurrentMoneySum, moneyCounterDuration, (money) =>
            {
                totalMoneySum.text = money.ToString();

                if (money > moneyBounceStep * count)
                {
                    count++;
                    PunchEffect.Punch(moneyImageTransform, moneyCounterImageScale, scaleMoneyImageDuraiton);
                }
            }).SetEase(Ease.OutExpo).Play();
        }

        private void StopCountingMoney()
        {
            if (moneyCounterTextTween.IsActive())
            {
                moneyCounterTextTween?.Kill();
            }
        }
    }
}