using UnityEngine;

class VectorUtility
{
    public static Vector3 ToVector3(float x, float y)
    {
        return new Vector3(x, y, 0.0f);
    }

    public static Vector3 ToVector3(Vector2 vector2)
    {
        return new Vector3(vector2.x, vector2.y, 0.0f);
    }

    public static Vector3 ToVector3(Vector3 vector3)
    {
        return new Vector3(vector3.x, vector3.y, vector3.z);
    }

    public static Vector2 ToVector2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }

    public static Vector3 Flatten(Vector3 vector3)
    {
        return new Vector3(vector3.x, vector3.y, 0.0f);
    }

    public static bool IsZero(Vector2 vector)
    {
        return Mathf.Approximately(vector.x, 0.0f) && Mathf.Approximately(vector.y, 0.0f);
    }
}