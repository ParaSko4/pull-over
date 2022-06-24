using Cinemachine;
using DG.Tweening;
using PullOver.Gameplay.Cars.Player;
using System;
using UnityEngine;

namespace PullOver.Gameplay.Camera
{
    public class CameraController : MonoBehaviour, IResetComponent
    {
        public event Action CameraUnfollowed;

        [SerializeField]
        private float shakeTime;
        [SerializeField]
        private float amplitudeGain;

        private PlayerCarBehaviour playerCarBehaviour;
        private CinemachineVirtualCamera cinemachineVirtualCamera;
        private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

        private Vector3 startPosition;

        private float currentAmplitudeGain;

        private void Awake()
        {
            playerCarBehaviour = FindObjectOfType<PlayerCarBehaviour>();
            cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            startPosition = cinemachineVirtualCamera.transform.position;
        }

        private void Start()
        {
            ResetComponent();
        }

        public void ResetComponent()
        {
            cinemachineVirtualCamera.transform.position = startPosition;
            cinemachineVirtualCamera.Follow = playerCarBehaviour.transform;
        }

        public void Shake()
        {
            CameraUnfollowed?.Invoke();

            cinemachineVirtualCamera.Follow = null;

            DOVirtual.Float(currentAmplitudeGain, amplitudeGain, shakeTime, (newAmplitudeGain) =>
            {
                currentAmplitudeGain = newAmplitudeGain;

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = currentAmplitudeGain;
            }).OnComplete(() =>
            {
                currentAmplitudeGain = 0f;
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            });
        }
    }
}
