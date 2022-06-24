using PullOver.Gameplay.Cars.SO;
using PullOver.Gameplay.Road;
using UnityEngine;
using Zenject;

namespace Assets.Content.DI
{
    public class CarsInstaller : MonoInstaller
    {
        [SerializeField]
        private CarsSettings carsSettings;
        [SerializeField]
        private Route routePrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(carsSettings);
            Container.Bind<Route>().FromComponentInNewPrefab(routePrefab).AsSingle();
        }
    }
}
