using UniRx.Toolkit;
using UnityEngine;

namespace PullOver.Gameplay.VFX
{
    public class VfxPool : ObjectPool<ParticleSystem>
    {
        private ParticleSystem instancePrefab;
        private float scaleFactor = 1f;

        public VfxPool(ParticleSystem instancePrefab)
        {
            this.instancePrefab = instancePrefab;
        }

        public VfxPool(ParticleSystem instancePrefab, float scaleFactor)
        {
            this.instancePrefab = instancePrefab;
            this.scaleFactor = scaleFactor;
        }

        protected override ParticleSystem CreateInstance()
        {
            ParticleSystem instanse = GameObject.Instantiate(instancePrefab);
            instanse.transform.localScale *= scaleFactor;
            instanse.Stop();

            return instanse;
        }
    }
}