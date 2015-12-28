using System.Collections.Generic;
using UnityEngine;

struct DelayedGizmoLine
{
    public Vector3 start;
    public Vector3 end;

    public DelayedGizmoLine(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }

    public DelayedGizmoLine(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }
}

class GizmosExtension
{
    public static List<DelayedGizmoLine> DelayedGizmoLines = new List<DelayedGizmoLine>();

    public static void DrawDelayedGizmos()
    {
        for (int i = 0; i < DelayedGizmoLines.Count; ++i)
        {
            Gizmos.DrawLine(DelayedGizmoLines[i].start, DelayedGizmoLines[i].end);
        }

        if (TimeHelper.GameTime > 0.0f)
        {
            DelayedGizmoLines.Clear();
        }
    }
}