using PullOver.Gameplay.Cars.Player;
using System;
using UnityEngine;

namespace PullOver.Gameplay.Level
{
    [RequireComponent(typeof(BoxCollider))]
    public class LevelCompletion : MonoBehaviour
    {
        public event Action PlayerInZone;

        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerCarBehaviour playerCarBehaviour;

            if (other.TryGetComponent(out playerCarBehaviour))
            {
                PlayerInZone?.Invoke();

                playerCarBehaviour.SmoothStop();
            }
        }
    }
}