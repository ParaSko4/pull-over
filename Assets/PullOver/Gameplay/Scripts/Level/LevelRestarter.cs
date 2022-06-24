using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace PullOver.Gameplay.Level
{
    public class LevelRestarter : MonoBehaviour
    {
        private struct StartTransform
        {
            public Rigidbody body;
            public Vector3 position;
            public Quaternion rotation;

            public StartTransform(Rigidbody body, Vector3 position, Quaternion rotation)
            {
                this.body = body;
                this.position = position;
                this.rotation = rotation;
            }
        }

        [SerializeField]
        private Transform parentOfInteractableObjects;

        private Dictionary<Transform, StartTransform> startingTransforms = new Dictionary<Transform, StartTransform>();
        private IResetComponent[] resetComponents;

        private void Awake()
        {
            resetComponents = FindObjectsOfType<MonoBehaviour>().OfType<IResetComponent>().ToArray();

            SavePositions();
        }

        public void ResetComponent()
        {
            ResetTransformsToDefaultPosition();

            foreach (var component in resetComponents)
            {
                component.ResetComponent();
            }
        }

        private void SavePositions()
        {
            foreach (Transform interactable in parentOfInteractableObjects)
            {
                startingTransforms[interactable] = new StartTransform(interactable.GetComponent<Rigidbody>(),
                    interactable.position,
                    interactable.rotation);
            }
        }

        private async void ResetTransformsToDefaultPosition()
        {
            List<Task> tasks = new List<Task>();

            foreach (Transform interactable in parentOfInteractableObjects)
            {
                tasks.Add(ResetObject(interactable));
            }

            await Task.WhenAll(tasks);
        }

        private async Task ResetObject(Transform transformObject)
        {
            startingTransforms[transformObject].body.velocity = Vector3.zero;
            transformObject.SetPositionAndRotation(startingTransforms[transformObject].position, startingTransforms[transformObject].rotation);

            await transformObject.DOMove(startingTransforms[transformObject].position, 0f).AsyncWaitForCompletion();
        }
    }
}