using UnityEngine;

class PhysicsHelper
{
    public static RaycastHit2D Physics2DRaycast(Vector3 start, Vector3 dir, float length, int layerMask)
    {
        GizmosExtension.DelayedGizmoLines.Add(new DelayedGizmoLine(start, start + dir * length));

        return Physics2D.Raycast(start, dir, length, layerMask);
    }
}