using UnityEngine;

public class UMath
{
    public static float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;

        if (result < 0)
        {
            result += 360f;
        }

        return result;
    }

    public static bool RotationStillWithinAngle(Vector3 rotation, float minAngle, float maxAngle)
    {
        if (minAngle > maxAngle)
        {
            return rotation.y > maxAngle && rotation.y < minAngle ? false : true;
        }

        return rotation.y > minAngle && rotation.y < maxAngle;
    }
}