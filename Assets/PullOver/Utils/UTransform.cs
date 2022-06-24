using UnityEngine;

public class UTransform
{
    private const float rotationLerpT = 0.5f;

    private static Quaternion nextRotationAngle;

    public static void RotateTransform(Transform rotationTransform, float rotationSpeed)
    {
        nextRotationAngle = Quaternion.Euler(rotationTransform.eulerAngles.x, rotationTransform.eulerAngles.y + rotationSpeed, rotationTransform.eulerAngles.z);
        rotationTransform.rotation = Quaternion.Lerp(rotationTransform.rotation, nextRotationAngle, rotationLerpT);
    }
}
