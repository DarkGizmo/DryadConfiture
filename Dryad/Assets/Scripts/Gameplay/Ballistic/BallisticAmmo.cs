using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BallisticAmmo : MonoBehaviour {

    private GameObject mOwner;
    public GameObject Explosion;
    public float ExplosionRadius = 4.0f;

    public int NumberOfHitBeforeExplosion = 1;
    public float BounceForce = 200.0f;

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
        float explosionRadius = ExplosionRadius;

        --NumberOfHitBeforeExplosion;
        if (NumberOfHitBeforeExplosion == 0)
        {
            DestroyObject(gameObject);
        }
        else
        {
            explosionRadius *= 0.5f;
        }


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

            float minX = hitLocation.x - explosionRadius;
            float maxX = hitLocation.x + explosionRadius;

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i].x >= minX && path[i].x <= maxX)
                {
                    float distX = Mathf.Abs(hitLocation.x - path[i].x);
                    float ratioX = distX / explosionRadius;

                    float height = Mathf.Sin((ratioX * 0.5f + 0.5f) * Mathf.PI) * explosionRadius;

                    float deltaDig = Mathf.Max((hitLocation.y + height) - path[i].y, 0.0f);
                    float deltaRemove = height * 2.0f - deltaDig;

                    if(NumberOfHitBeforeExplosion == 0)
                    {
                        path[i].y += Mathf.Max(deltaRemove, 0.0f);
                    }
                    else
                    {
                        path[i].y -= Mathf.Max(deltaRemove, 0.0f);
                    }
                }
            }

            //Apply deformation
            terrainEditor2D.ApplyDeform(path, true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        OnImpact();

        Vector2 impactNormal = Vector2.zero;
        for (int i = 0; i < collision.contacts.Length; ++i)
        {
            impactNormal += collision.contacts[i].normal;
        }

        impactNormal.Normalize();

        if(NumberOfHitBeforeExplosion > 0)
        {
            Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.AddForce(impactNormal * BounceForce);
        }

        GameObject go = (GameObject)Instantiate(Explosion, transform.position, Quaternion.LookRotation(impactNormal, Vector2.up));
        Destroy(go, 1.0f);
    }
}
