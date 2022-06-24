using DG.Tweening;
using GUI.Base;
using UnityEngine;

namespace PullOver.GUI.Screens.Video.Buttons
{
    [RequireComponent(typeof(CanvasGroup))]
    public class RestartButton : BaseButton
    {
        [SerializeField]
        private float shownTime;

        private CanvasGroup fade;

        protected override void OnAwake()
        {
            fade = GetComponent<CanvasGroup>();
            fade.alpha = 0f;
        }

        public void Show()
        {
            fade.DOFade(1f, shownTime);
        }

        public void Hide()
        {
            fade.DOFade(0f, shownTime).OnComplete(() => gameObject.SetActive(false));
        }
    }
}