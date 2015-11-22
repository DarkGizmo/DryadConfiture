

using UnityEngine;
using System.Collections.Generic;

class Poly2DUtility
{
    public static void DrawCircle(Vector3 center, float radius, Material material)
    {
        DrawCircle(center, radius, material, 1.0f);
    }

    public static void DrawCircle(Vector3 center, float radius, Material material, float ratio)
    {
        DrawCircle(center, radius, material, ratio, 12);
    }

    public static void DrawCircle(Vector3 center, float radius, Material material, float ratio, int steps)
    {
        DrawCircle(center, radius, material, ratio, steps, true);
    }

    public static void DrawCircle(Vector3 center, float radius, Material material, float ratio, int steps, bool clockWise)
    {
        if (clockWise)
        {
            DrawClockwiseCircle(center, radius, material, ratio, steps);
        }
        else
        {
            DrawCounterClockwiseCircle(center, radius, material, ratio, steps);
        }
    }

    public static void DrawClockwiseCircle(Vector3 center, float radius, Material material, float ratio, int steps)
    {
        material.SetPass(0);

        GL.Begin(GL.TRIANGLE_STRIP);

        float minRatio = (1.0f - ratio);

        for (int i = Mathf.FloorToInt(minRatio * (float)(steps - 1)); i < steps; ++i)
        {
            float currentRatio = Mathf.Max(((float)i /(float)(steps - 1)), minRatio);
            
            const float BASE_ANGLE = -(Mathf.PI * 0.5f);

            Vector2 circleOffset = new Vector2(Mathf.Cos(BASE_ANGLE + currentRatio * -MathUtility.TAU), Mathf.Sin(BASE_ANGLE + currentRatio * -MathUtility.TAU));
            GL.TexCoord((circleOffset + Vector2.one) * 0.5f);
            GL.Vertex(center + radius * VectorUtility.ToVector3(circleOffset));

            GL.TexCoord2(0.5f, 0.5f);
            GL.Vertex(center);
        }
        GL.End();
    }

    public static void DrawCounterClockwiseCircle(Vector3 center, float radius, Material material, float ratio, int steps)
    {
        material.SetPass(0);

        GL.Begin(GL.TRIANGLE_STRIP);
        
        float minRatio = (1.0f - ratio);
        
        for (int i = Mathf.FloorToInt(minRatio * (float)(steps - 1)); i < steps; ++i)
        {
            float currentRatio = Mathf.Max(((float)i /(float)(steps - 1)), minRatio);
            
            const float BASE_ANGLE = -(Mathf.PI * 0.5f);

            GL.TexCoord2(0.5f, 0.5f);
            GL.Vertex(center);

            Vector2 circleOffset = new Vector2(Mathf.Cos(BASE_ANGLE + currentRatio * MathUtility.TAU), Mathf.Sin(BASE_ANGLE + currentRatio * MathUtility.TAU));
            GL.TexCoord((circleOffset + Vector2.one) * 0.5f);
            GL.Vertex(center + radius * VectorUtility.ToVector3(circleOffset));
        }
        
        GL.End();
    }
}