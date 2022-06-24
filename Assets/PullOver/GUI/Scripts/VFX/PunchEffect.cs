using DG.Tweening;
using UnityEngine;

namespace PullOver.GUI.VFX
{
    public class PunchEffect
    {
        public static void Punch(Transform transform, float scale, float duration)
        {
            transform.DORewind();
            transform.DOPunchScale(new Vector3(scale, scale, scale), duration)
                .SetEase(Ease.OutExpo);
        }
    }
}