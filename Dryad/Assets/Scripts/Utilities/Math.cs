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

    public static float Unlerp(float value, float minValue, float maxValue)
    {
        return (Mathf.Clamp(value, minValue, maxValue) - minValue) / (maxValue - minValue);
    }

    public static float UnlerpClamped(float value, float minValue, float maxValue)
    {
        return Unlerp(Mathf.Clamp(value, minValue, maxValue), minValue, maxValue);
    }

    public static float MapClamped(float value, float minInRange, float maxInRange, float minOutRange, float maxOutRange)
    {
        float ratio = UnlerpClamped(value, minInRange, maxInRange);
        return Mathf.Lerp(minOutRange, maxOutRange, ratio);
    }
}
