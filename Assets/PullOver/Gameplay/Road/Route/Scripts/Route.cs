using PullOver.Utils;
using UnityEngine;

namespace PullOver.Gameplay.Road
{
    public class Route : MonoBehaviour
    {
        [SerializeField]
        private AnimationCurve accelerationCurve;
        [SerializeField]
        private float travelTime;

        private Transform[] points = new Transform[4];

        private Vector3[] startPointPositions = new Vector3[4];

        public AnimationCurve Curve
        {
            get
            {
                return accelerationCurve;
            }
        }

        public float TravelTime
        {
            get
            {
                return travelTime;
            }
        }

        private void Awake()
        {
            int i = 0;

            foreach (Transform child in transform)
            {
                startPointPositions[i] = child.localPosition;
                points[i] = child;

                i++;
            }
        }

        public Vector3[] GetAlongPath(Transform movableTransform)
        {
            Vector3[] cachedPointPositions = new Vector3[4];

            cachedPointPositions[0] = movableTransform.position;

            for (int i = 1; i < cachedPointPositions.Length; i++)
            {
                cachedPointPositions[i] = movableTransform.position + points[i].localPosition;
            }

            return cachedPointPositions;
        }

        public Vector3[] GetAlongMirrorPath(Transform movableTransform)
        {
            Vector3[] cachedPointPositions = new Vector3[4];

            cachedPointPositions[0] = movableTransform.localPosition;

            for (int i = 1; i < cachedPointPositions.Length; i++)
            {
                cachedPointPositions[i] = movableTransform.position + new Vector3(-points[i].localPosition.x, points[i].localPosition.y, points[i].localPosition.z);
            }

            return cachedPointPositions;
        }

        public Vector3[] GetAlongPathOnCrossRoad(Transform movableTransform, Vector3 crossTurn)
        {
            Vector3[] cachedPointPositions = new Vector3[4];

            cachedPointPositions[0] = movableTransform.position;
            cachedPointPositions[1] = movableTransform.position + points[1].localPosition;
            cachedPointPositions[2] = new Vector3(points[1].localPosition.x, movableTransform.position.y + points[2].localPosition.y, crossTurn.z);
            cachedPointPositions[3] = crossTurn;

            return cachedPointPositions;
        }

        private void OnDrawGizmos()
        {
            Transform[] pointsGizmos = new Transform[4];

            int i = 0;

            foreach (Transform child in transform)
            {
                pointsGizmos[i] = child;
                i++;
            }

            int segments = 20;

            Vector3 prevPoint = pointsGizmos[0].position;

            for (i = 0; i < segments + 1; i++)
            {
                float param = (float)i / segments;

                Vector3 point = Bezier.GetPoint(pointsGizmos[0].position, pointsGizmos[1].position, pointsGizmos[2].position, pointsGizmos[3].position, param);

                Gizmos.DrawLine(prevPoint, point);

                prevPoint = point;
            }

            float sphereSize = 0.3f;

            Gizmos.DrawSphere(pointsGizmos[0].position, sphereSize);
            Gizmos.DrawSphere(pointsGizmos[1].position, sphereSize);
            Gizmos.DrawSphere(pointsGizmos[2].position, sphereSize);
            Gizmos.DrawSphere(pointsGizmos[3].position, sphereSize);
        }
    }
}