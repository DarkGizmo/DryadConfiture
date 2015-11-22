using UnityEngine;

class CameraUtility
{
    public static float GetHorizontalFov(Camera camera)
    {
        float verticalFOVRad = camera.fieldOfView * Mathf.Deg2Rad;
        float cameraHeightAtOneUnit = Mathf.Tan(verticalFOVRad * 0.5f);
        float horizontalFOVRad = Mathf.Atan(cameraHeightAtOneUnit * camera.aspect) * 2.0f;
        
        return horizontalFOVRad * Mathf.Rad2Deg;
    }

    public static float GetHorizontalRadFov(Camera camera)
    {
        float verticalFOVRad = camera.fieldOfView * Mathf.Deg2Rad;
        float cameraHeightAtOneUnit = Mathf.Tan(verticalFOVRad * 0.5f);
        float horizontalFOVRad = Mathf.Atan(cameraHeightAtOneUnit * camera.aspect) * 2.0f;
        
        return horizontalFOVRad;
    }

    public static float GetHalfRadHorizontalFov(Camera camera)
    {
        return GetHorizontalRadFov(camera) * 0.5f;
    }

    public static float GetVerticalFov(Camera camera)
    {
        return camera.fieldOfView;
    }

    public static float GetRadVerticalFov(Camera camera)
    {
        return camera.fieldOfView * Mathf.Deg2Rad;
    }

    public static float GetHalfRadVerticalFov(Camera camera)
    {
        return GetRadVerticalFov(camera) * 0.5f;
    }

    public static Vector2 GetWorldSizeAtDistance(Camera camera, float distance)
    {
        float fullHorizontalFov = GetHorizontalFov(camera);
        float fullVerticalFov = GetVerticalFov(camera);
        Vector2 frustumSize2dAtOrigin = new Vector2(Mathf.Tan(fullHorizontalFov * Mathf.Deg2Rad * 0.5f) * distance, Mathf.Tan(fullVerticalFov * Mathf.Deg2Rad * 0.5f) * distance) * 2.0f;
        
        return frustumSize2dAtOrigin;
    }
}
