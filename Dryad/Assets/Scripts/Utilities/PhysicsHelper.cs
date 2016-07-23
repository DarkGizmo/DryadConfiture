using UnityEngine;

class PhysicsHelper
{
    public static RaycastHit2D Physics2DRaycast(Vector3 start, Vector3 dir, float length, int layerMask)
    {
        return Physics2DRaycast(start, dir, length, layerMask, false);
    }

    public static RaycastHit2D Physics2DRaycast(Vector3 start, Vector3 dir, float length, int layerMask, bool drawGizmosNow)
    {
        if(drawGizmosNow)
        {
            Gizmos.DrawLine(start, start + dir * length);
        }
        else
        {
            GizmosExtension.DelayedGizmoLines.Add(new DelayedGizmoLine(start, start + dir * length));
        }

        return Physics2D.Raycast(start, dir, length, layerMask);
    }
}