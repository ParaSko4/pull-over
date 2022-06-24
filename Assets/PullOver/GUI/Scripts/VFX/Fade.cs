using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

namespace PullOver.GUI.VFX
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Fade : MonoBehaviour
    {
        [SerializeField]
        private float fadeDuration;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public async Task Show()
        {
            canvasGroup.blocksRaycasts = false;

            await canvasGroup.DOFade(1f, fadeDuration).AsyncWaitForCompletion();

            canvasGroup.blocksRaycasts = true;
        }

        public async Task Hide()
        {
            canvasGroup.blocksRaycasts = true;

            await canvasGroup.DOFade(0f, fadeDuration).AsyncWaitForCompletion();

            canvasGroup.blocksRaycasts = false;
        }

        public void ShowInstantly()
        {
            canvasGroup.alpha = 1f;
        }

        public void HideInstantly()
        {
            canvasGroup.alpha = 0f;
        }
    }
}
