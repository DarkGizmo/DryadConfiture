
using UnityEngine;
using System.Collections.Generic;

class LineUtility
{
    public static void DrawLine(Vector3 pointA, Vector3 pointB, Color color)
    {
        GL.Color(color);
        GL.Begin(GL.LINES);
        
        GL.Vertex3(pointA.x, pointA.y, pointA.z);
        GL.Vertex3(pointB.x, pointB.y, pointB.z);
        
        GL.End();
    }

    public static void DrawLines(IList<Vector3> points, Color color)
    {
        if (points.Count > 1)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);

            for (int i = 0; i < points.Count - 1; ++i)
            {
                GL.Vertex3(points [i].x, points [i].y, points [i].z);
                GL.Vertex3(points [i+1].x, points [i+1].y, points [i+1].z);
            }
        
            GL.End();
        }
    }

    public static void DrawLine(Vector3 pointA, Vector3 pointB, Material material)
    {
        material.SetPass(0);
        GL.Begin(GL.LINES);

        GL.Vertex3(pointA.x, pointA.y, pointA.z);
        GL.Vertex3(pointB.x, pointB.y, pointB.z);
        
        GL.End();
    }
    
    public static void DrawLines(IList<Vector3> points, Material material)
    {
        if (points.Count > 1)
        {
            material.SetPass(0);
            GL.Begin(GL.LINES);
        
            for (int i = 0; i < points.Count - 1; ++i)
            {
                GL.Vertex3(points [i].x, points [i].y, points [i].z);
                GL.Vertex3(points [i+1].x, points [i+1].y, points [i+1].z);
            }
        
            GL.End();
        }
    }

    public static void DrawLine(Vector3 pointA, Vector3 pointB, float width, Color color)
    {
        GL.Begin(GL.QUADS);
        GL.Color(color);
        
        float halfLineWidth = width * 0.5f;
        
        Vector3 normal = -Camera.main.transform.forward;
        Vector3 side = Vector3.Cross(normal, pointB-pointA);
        side.Normalize(); 
        
        Vector3 a = pointA + side * halfLineWidth;
        Vector3 b = pointA - side * halfLineWidth;
        Vector3 c = pointB + side * halfLineWidth;
        Vector3 d = pointB - side * halfLineWidth;
        
        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(a.x, a.y, a.z);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(b.x, b.y, b.z);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(d.x, d.y, d.z);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(c.x, c.y, c.z);
        
        GL.End();
    }

    public static void DrawLines(IList<Vector3> points, float width, Color color)
    {
        if (points.Count > 1)
        {
            GL.Begin(GL.QUADS);
            GL.Color(color);
            {
                float halfLineWidth = width * 0.5f;
                Vector3 normal = -Camera.main.transform.forward;
                Vector3 lastSide = Vector3.Cross(normal, points[1] - points[0]);
                lastSide.Normalize();

                for (int i = 0; i < points.Count - 1; ++i)
                {
                    Vector3 pointA = points[i];
                    Vector3 pointB = points[i+1];

                    Vector3 side = Vector3.Cross(normal, pointB - pointA);
                    side.Normalize(); 
            
                    Vector3 a = pointA + lastSide * halfLineWidth;
                    Vector3 b = pointA - lastSide * halfLineWidth;
                    Vector3 c = pointB + side * halfLineWidth;
                    Vector3 d = pointB - side * halfLineWidth;
            
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3(a.x, a.y, a.z);
                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3(b.x, b.y, b.z);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex3(d.x, d.y, d.z);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3(c.x, c.y, c.z);

                    lastSide = side;
                }
            }
            GL.End();
        }
    }

    public static void DrawLine(Vector3 pointA, Vector3 pointB, float width, Material material)
    {
        material.SetPass(0);
        GL.Begin(GL.QUADS);

        float halfLineWidth = width * 0.5f;
        
        Vector3 normal = -Camera.main.transform.forward;
        Vector3 side = Vector3.Cross(normal, pointB-pointA);
        side.Normalize(); 
        
        Vector3 a = pointA + side * halfLineWidth;
        Vector3 b = pointA - side * halfLineWidth;
        Vector3 c = pointB + side * halfLineWidth;
        Vector3 d = pointB - side * halfLineWidth;
        
        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex(a);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex(b);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex(d);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex(c);
        
        GL.End();
    }
    
    public static void DrawLines(IList<Vector3> points, float width, Material material)
    {
        if (points.Count > 1)
        {
            material.SetPass(0);
            GL.Begin(GL.QUADS);
            {
                float halfLineWidth = width * 0.5f;
                Vector3 normal = -Camera.main.transform.forward;
                Vector3 lastSide = Vector3.Cross(normal, points[1] - points[0]);
                lastSide.Normalize();
                
                for (int i = 0; i < points.Count - 1; ++i)
                {
                    Vector3 pointA = points[i];
                    Vector3 pointB = points[i+1];
                    
                    Vector3 side = Vector3.Cross(normal, pointB - pointA);
                    side.Normalize(); 
                    
                    Vector3 a = pointA + lastSide * halfLineWidth;
                    Vector3 b = pointA - lastSide * halfLineWidth;
                    Vector3 c = pointB + side * halfLineWidth;
                    Vector3 d = pointB - side * halfLineWidth;
                    
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3(a.x, a.y, a.z);
                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3(b.x, b.y, b.z);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex3(d.x, d.y, d.z);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3(c.x, c.y, c.z);
                    
                    lastSide = side;
                }
            }
            GL.End();
        }
    }
}