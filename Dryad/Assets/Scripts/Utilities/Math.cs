using UnityEngine;

public class MathUtility
{
    public static float EPSILON = Mathf.Epsilon;
    public static float TAU = Mathf.PI * 2.0f;

    public static bool IsEqualWithEpsilon(float a, float b)
    {
        return IsEqualWithEpsilon(a, b, EPSILON);
    }

    public static bool IsEqualWithEpsilon(float a, float b, float epsilon)
    {
        return Mathf.Abs(a - b) < epsilon;
    }
}
