using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BallisticAmmo : MonoBehaviour {

    private GameObject mOwner;

    public float ExplosionRadius = 4.0f;

    public void Initialize(GameObject owner)
    {
        mOwner = owner;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    virtual public void OnImpact()
    {
        DestroyObject(gameObject);

        GameObject[] allTerrains = GameObject.FindGameObjectsWithTag("Terrain");

        foreach (var terrainEditor2DObject in allTerrains)
        {
            TerrainEditor2D terrainEditor2D = terrainEditor2DObject.GetComponent<TerrainEditor2D>();

            if(terrainEditor2D == null)
            {
                continue;
            }

            Vector3 hitLocation = transform.position - terrainEditor2D.transform.position;

            //Get array of points of terrain path in local space
            Vector3[] path = terrainEditor2D.GetPath(Space.Self);

            float minX = hitLocation.x - ExplosionRadius;
            float maxX = hitLocation.x + ExplosionRadius;

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i].x >= minX && path[i].x <= maxX)
                {
                    float distX = Mathf.Abs(hitLocation.x - path[i].x);
                    float ratioX = distX / ExplosionRadius;

                    float height = Mathf.Sin((ratioX * 0.5f + 0.5f) * Mathf.PI) * ExplosionRadius;

                    float deltaDig = Mathf.Max((hitLocation.y + height) - path[i].y, 0.0f);
                    float deltaRemove = height * 2.0f - deltaDig;

                    path[i].y -= Mathf.Max(deltaRemove, 0.0f);
                }
            }

            //Apply deformation
            terrainEditor2D.ApplyDeform(path, true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        OnImpact();
    }
}
