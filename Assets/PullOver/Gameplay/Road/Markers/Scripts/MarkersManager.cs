using UnityEngine;

namespace PullOver.Gameplay.Road.Markers
{
    public class MarkersManager : MonoBehaviour, IResetComponent
    {
        private SpawnMarker[] carMarkers;

        public int MarkersCount
        {
            get
            {
                return carMarkers.Length;
            }
        }

        private void Awake()
        {
            carMarkers = FindObjectsOfType<SpawnMarker>();
        }

        public void ResetComponent()
        {
            for (int i = 0; i < carMarkers.Length; i++)
            {
                carMarkers[i].gameObject.SetActive(true);
            }
        }
    }
}