using PullOver.Gameplay;
using PullOver.Gameplay.Level.Values.Money;
using PullOver.GUI.VFX;
using TMPro;
using UnityEngine;

namespace PullOver.GUI.Screens.Status
{
    public class MoneyPresenter : MonoBehaviour, IResetComponent
    {
        [SerializeField]
        private Transform moneyImageTransform;
        [SerializeField]
        private float moneyCounterImageScale;
        [SerializeField]
        private float scaleMoneyImageDuraiton;
        [SerializeField]
        private int moneyBounceStep;

        private TextMeshProUGUI moneyCounterResult;
        private MoneyCounter moneyCounter;

        private int count = 0;

        private void Awake()
        {
            moneyCounterResult = GetComponentInChildren<TextMeshProUGUI>();
            moneyCounter = FindObjectOfType<MoneyCounter>();

            moneyCounter.MoneyChange += OnMoneyChange;
        }

        private void OnDestroy()
        {
            moneyCounter.MoneyChange -= OnMoneyChange;
        }

        public void ResetComponent()
        {
            count = 0;
            moneyCounterResult.text = 0.ToString();
        }

        private void OnMoneyChange(int money)
        {
            moneyCounterResult.text = money.ToString();

            if (money > moneyBounceStep * count)
            {
                count++;
                PunchEffect.Punch(moneyImageTransform, moneyCounterImageScale, scaleMoneyImageDuraiton);
            }
        }
    }
}
