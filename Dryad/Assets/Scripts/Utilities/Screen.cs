using UnityEngine;

class ScreenUtility
{
    public static float GetHorizontalFOV()
    {
        float vFOVrad = Camera.main.fieldOfView * Mathf.Deg2Rad;
        float cameraHeightAt1 = Mathf.Tan(vFOVrad *0.5f);
        float hFOVrad = Mathf.Atan(cameraHeightAt1 * Camera.main.aspect) * 2;

        return hFOVrad * Mathf.Rad2Deg;
    }

    public static Vector2 GetMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.y = Screen.height - mousePosition.y;

        return mousePosition;
    }

    public static Vector3 GUIToScreenPosition(Vector3 guiPosition)
    {
        return new Vector3(guiPosition.x, Screen.height - guiPosition.y);
    }

    public static Vector3 ToScreenPosition(Vector3 worldPosition)
    {
        Vector3 projectedPosition = -0.01f * (Camera.main.projectionMatrix * worldPosition);
        projectedPosition.x *= Screen.width;
        projectedPosition.y *= Screen.height;
        
        return projectedPosition + new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
    }

    public static Rect MergeRect(Rect a, Rect b)
    {
        Rect merge = new Rect();

        merge.xMin = Mathf.Min(a.xMin, b.xMin);
        merge.yMin = Mathf.Min(a.yMin, b.yMin);
        merge.xMax = Mathf.Max(a.xMax, b.xMax);
        merge.yMax = Mathf.Max(a.yMax, b.yMax);

        return merge;
    }
}
