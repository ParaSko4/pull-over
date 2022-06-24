using UnityEngine;

namespace PullOver.Utils
{
    public static class Bezier
    {
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float t1 = 1f - t;

            return t1 * t1 * t1 * p0 + 
                3f * t1 * t1 * t * p1 + 
                3f * t1 * t * t * p2 + 
                t * t * t * p3;
        }

        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float t1 = 1f - t;

            return 3f * t1 * t1 * (p1 - p0) +
                6f * t1 * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }
    }
}
