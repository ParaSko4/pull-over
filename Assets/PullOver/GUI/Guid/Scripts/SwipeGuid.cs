using Cysharp.Threading.Tasks;
using DG.Tweening;
using PullOver.Gameplay;
using PullOver.Gameplay.Cars;
using PullOver.Gameplay.Cars.Enum;
using PullOver.Gameplay.Cars.Player;
using PullOver.Gameplay.Cars.SO;
using PullOver.Gameplay.Level.Values;
using UnityEngine;
using Zenject;

namespace PullOver.GUI.Guid
{
    public class SwipeGuid : MonoBehaviour
    {
        [SerializeField]
        private Transform hand;
        [SerializeField]
        private Transform start;
        [SerializeField]
        private Transform leftSwipe;
        [SerializeField]
        private float timeHandMovement;
        [SerializeField]
        private float timeHandShown;
        [SerializeField]
        private TurningSide turningSide;

        private Timer timer;
        private CanvasGroup handFade;
        private CarsManager carsManager;
        private CarsSettings carsSettings;
        private Sequence handMovementSequence;
        private InputController inputController;
        private PlayerCarBehaviour playerCarBehaviour;
        private CarsSwipeBehaviour carsSwipeBehaviour;

        private Vector3 leftSwipePosition;
        private Vector3 rightSwipePosition;

        private bool isGuidEnd;

        [Inject]
        public void Construct(CarsSettings carsSettings)
        {
            this.carsSettings = carsSettings;
        }

        private async void Awake()
        {
            handFade = hand.GetComponent<CanvasGroup>();
            timer = FindObjectOfType<Timer>();
            carsManager = FindObjectOfType<CarsManager>();
            inputController = FindObjectOfType<InputController>();
            carsSwipeBehaviour = FindObjectOfType<CarsSwipeBehaviour>();
            playerCarBehaviour = FindObjectOfType<PlayerCarBehaviour>();

            inputController.Swiped += OnSwiped;

            leftSwipePosition = leftSwipe.localPosition;
            rightSwipePosition = new Vector3(-leftSwipe.localPosition.x, leftSwipe.localPosition.y, leftSwipe.localPosition.z);

            hand.localPosition = start.localPosition;
            hand.gameObject.SetActive(true);
            handFade.alpha = 0f;

            carsSwipeBehaviour.StopReacting = true;
            timer.StopTimer();

            await UniTask.Delay(1000);

            await handFade.DOFade(1f, timeHandShown).AsyncWaitForCompletion();

            await UniTask.WaitUntil(() => carsSwipeBehaviour.FirstCarForSwipe != null);

            if (isGuidEnd)
            {
                return;
            }

            carsSwipeBehaviour.FirstCarForSwipe.CarController.UpdateSpeedSmooth(0f, 1f);
            playerCarBehaviour.CarController.UpdateSpeedSmooth(0f, 1f);
            handMovementSequence = DOTween.Sequence()
                   .Append(hand.DOLocalMove(turningSide == TurningSide.Left ? leftSwipePosition : rightSwipePosition, timeHandMovement))
                   .Append(hand.DOLocalMove(start.localPosition, timeHandMovement))
                   .Play().SetLoops(-1);
        }

        private void OnSwiped(TurningSide inputTurningSide)
        {
            if (inputTurningSide != turningSide)
            {
                return;
            }

            isGuidEnd = true;

            timer.StartTimer();
            carsSwipeBehaviour.Swipe(inputTurningSide);

            carsManager.MaxActiveCars = 2;
            inputController.Swiped -= OnSwiped;
            carsSwipeBehaviour.StopReacting = false;

            handMovementSequence?.Kill();
            handFade.DOKill();
            playerCarBehaviour.CarController.UpdateSpeedSmooth(carsSettings.PlayerMovementSpeed, 1f);
            handFade.DOFade(0f, timeHandShown).OnComplete(() =>
            {
                hand.gameObject.SetActive(false);
            });
        }
    }
}