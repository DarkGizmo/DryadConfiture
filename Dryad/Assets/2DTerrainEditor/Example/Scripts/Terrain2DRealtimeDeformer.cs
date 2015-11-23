/* This script shows how realtime deformation works. Read the comments in code to understand the process */

using System;
using System.Collections.Generic;
using UnityEngine;

//Rigidbody2D is required for detect collisions with 2D terrain
[RequireComponent(typeof(Rigidbody2D))]
public class Terrain2DRealtimeDeformer : MonoBehaviour
{
    public float DeformerHardness = 0.25f;
    public float DeformerNoise = 0;

    //List of 2D terrains which is currently collides with this deformer
    private List<TerrainEditor2D> _curTerrains = new List<TerrainEditor2D>();

    void OnTriggerEnter2D(Collider2D col)
    {
        //Add reference to list if deformer collides with terrain
        if (col.GetComponent<TerrainEditor2D>() && !_curTerrains.Contains(col.GetComponent<TerrainEditor2D>()))
            _curTerrains.Add(col.GetComponent<TerrainEditor2D>());
    }

    void OnTriggerExit2D(Collider2D col)
    {
    }

    void Update()
    {
        //Move deformer to mouse position
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);

        //Dig terrain if mouse button is pressed
        if (Input.GetMouseButtonDown(0))
            DeformTerrain(DeformMode.Dig);
        //Raise terrain if mouse button is pressed
        if (Input.GetMouseButtonDown(1))
            DeformTerrain(DeformMode.Raise);
        //Raise terrain if mouse button is pressed
        if (Input.GetMouseButtonDown(2))
            DeformTerrain(DeformMode.Remove);
    }

    private Vector3 GetMouseWorldPosition()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        (new Plane(-Camera.main.transform.forward, transform.position.z)).Raycast(ray, out distance);

        Vector3 position = ray.GetPoint(distance);

        return position;
    }

    void DeformTerrain(DeformMode deformMode)
    {
        //Check of deformer currently collides with any of terrain
        if (_curTerrains.Count == 0)
            return;

        float paintBrushRadius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
        float minX = transform.position.x - paintBrushRadius;
        float maxX = transform.position.x + paintBrushRadius;

        foreach (var terrainEditor2D in _curTerrains)
        {
            //Get array of points of terrain path in local space
            Vector3[] path = terrainEditor2D.GetPath(Space.Self);

            for (int i = 0; i < path.Length; i++)
            {
                //Check if collider overpals with any of path points
                if (deformMode != DeformMode.Remove)
                {
                    if (GetComponent<Collider2D>().OverlapPoint(path[i] + terrainEditor2D.transform.position))
                    {
                        if (deformMode == DeformMode.Dig)
                        {
                            path[i] -= new Vector3(0, (DeformerHardness));
                        }
                        else if (deformMode == DeformMode.Raise)
                        {
                            path[i] += new Vector3(0, (DeformerHardness));
                        }

                        //Apply noise of needed
                        if (DeformerNoise > 0f)
                            path[i] += new Vector3(0, UnityEngine.Random.Range(-DeformerNoise * 0.25f, DeformerNoise * 0.25f));
                    }
                }
                else if (path[i].x >= minX && path[i].x <= maxX)
                {
                    float distX = Mathf.Abs(GetMouseWorldPosition().x - path[i].x);
                    float ratioX = distX / paintBrushRadius;

                    float height = Mathf.Sin((ratioX * 0.5f + 0.5f) * Mathf.PI) * paintBrushRadius;

                    float deltaDig = Mathf.Max((GetMouseWorldPosition().y + height) - path[i].y, 0.0f);
                    float deltaRemove = height * 2.0f - deltaDig;

                    path[i].y -= Mathf.Max(deltaRemove, 0.0f);

                    //Apply noise of needed
                    if (DeformerNoise > 0f)
                        path[i] += new Vector3(0, UnityEngine.Random.Range(-DeformerNoise * 0.25f, DeformerNoise * 0.25f));
                }
            }

            //Apply deformation
            terrainEditor2D.ApplyDeform(path, true);
        }
    }
    
    [Serializable]
    public enum DeformMode
    {
        Dig, Raise, Remove
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 1.0f);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
