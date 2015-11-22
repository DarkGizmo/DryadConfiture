using UnityEngine;

public class DampingUtility
{
    public static float LerpDamp(float current, float desired, float ratio, float dt)
    {
        return Mathf.Lerp(current, desired, Mathf.Clamp01(ratio * dt));
    }

    public static float Damp(float current, float desired, float factor, float dt)
    {
        if (dt == 0.0f)
        {
            return current;
        }
        else if (factor == 0.0f)
        {
            return desired;
        }

        return (((current * factor) + (desired * dt)) / (factor + dt));
    }

    public static float SinSmooth(float current)
    {
        return (Mathf.Sin((current * Mathf.PI) - (Mathf.PI * 0.5f)) + 1.0f) * 0.5f;
    }
}
