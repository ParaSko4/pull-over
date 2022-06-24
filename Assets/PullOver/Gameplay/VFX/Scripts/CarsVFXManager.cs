using Cysharp.Threading.Tasks;
using UniRx.Toolkit;
using UnityEngine;

namespace PullOver.Gameplay.VFX
{
    public class CarsVfxManager : MonoBehaviour
    {
        private const int ThresholdMult = 2;

        [Header("Parking")]
        [Space]
        [SerializeField]
        private ParticleSystem parking;
        [SerializeField]
        private int parkingPoolCount;
        [Space]
        [SerializeField]
        private float parkingScale;
        [Header("Explosion")]
        [Space]
        [SerializeField]
        private ParticleSystem explosion;
        [SerializeField]
        private int explosionPoolCount;
        [Space]
        [SerializeField]
        private float explosionRadius;
        [SerializeField]
        private float explosionForce;
        [SerializeField]
        private float explosionScale;

        private VfxPool parkingPool;
        private VfxPool explosionPool;

        private async void Awake()
        {
            parkingPool = new VfxPool(parking, parkingScale);
            explosionPool = new VfxPool(explosion, explosionScale);
            await parkingPool.PreloadAsync(parkingPoolCount, parkingPoolCount * ThresholdMult).ToUniTask();
            await explosionPool.PreloadAsync(explosionPoolCount, explosionPoolCount * ThresholdMult).ToUniTask();
        }

        public void BlowUp(Vector3 explosionPosition)
        {
            Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

            foreach (var collider in colliders)
            {
                Rigidbody rb;

                if (collider.TryGetComponent(out rb))
                {
                    rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
                }
            }

            ParticleSystem explosionInstance = explosionPool.Rent();
            explosionInstance.transform.SetPositionAndRotation(explosionPosition, Quaternion.identity);
            explosionInstance.Play();

            ReturnAfterTime(explosionPool, explosionInstance);
        }

        public void Parked(Vector3 position)
        {
            ParticleSystem parkingInstance = parkingPool.Rent();
            parkingInstance.transform.position = position;

            ReturnAfterTime(parkingPool, parkingInstance);
        }

        private async void ReturnAfterTime(ObjectPool<ParticleSystem> pool, ParticleSystem particle)
        {
            await UniTask.Delay(5000);

            pool.Return(particle);
        }
    }
}
