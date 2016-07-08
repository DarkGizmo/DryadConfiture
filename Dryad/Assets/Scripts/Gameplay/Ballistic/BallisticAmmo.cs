using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BallisticAmmo : MonoBehaviour {

    private GameObject mOwner;
    public GameObject Explosion;
    public float ExplosionRadius = 4.0f;
    public float Damage = 100.0f;

    public int NumberOfHitBeforeExplosion = 1;
    public float BounceForce = 200.0f;
    public float DigFactor = 1.0f;

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

    private void ModifyTerrain(TerrainEditor2D terrain, Vector3 location, float radius, float digFactor)
    {
        Vector3 hitLocation = location - terrain.transform.position;

        //Get array of points of terrain path in local space
        Vector3[] path = terrain.GetPath(Space.Self, true);

        float minX = hitLocation.x - radius;
        float maxX = hitLocation.x + radius;

        for (int i = 0; i < path.Length; i++)
        {
            if (path[i].x >= minX && path[i].x <= maxX)
            {
                float distX = Mathf.Abs(hitLocation.x - path[i].x);
                float ratioX = distX / radius;

                float height = Mathf.Sin((ratioX * 0.5f + 0.5f) * Mathf.PI) * radius;

                float deltaDig = Mathf.Max((hitLocation.y + height) - path[i].y, 0.0f);
                float deltaRemove = height * 2.0f - deltaDig;

                float deltaMove = 0.0f;
                if (NumberOfHitBeforeExplosion == 0)
                {
                    deltaMove = -Mathf.Max(deltaRemove, 0.0f) * digFactor;
                }
                else
                {
                    deltaMove = Mathf.Max(deltaRemove, 0.0f) * digFactor;
                }

                // clamp if already higher then what we were gonna add
                if(deltaMove > 0.0f)
                {
                    if(path[i].y - hitLocation.y > deltaMove)
                    {
                        deltaMove = 0.0f;
                    }
                    else
                    {
                        deltaMove = Mathf.Min(deltaMove, Mathf.Max(path[i].y - hitLocation.y, 0.0f));
                    }
                }

                path[i].y += deltaMove;
            }
        }

        //Apply deformation
        terrain.ApplyDeform(path, true);
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

        for(int i = 0; i < GameplayObjectManager.Instance.TerrainList.Count; ++i)
        {
            TerrainEditor2D terrainEditor2D = GameplayObjectManager.Instance.TerrainList[i];

            if(terrainEditor2D == null)
            {
                continue;
            }

            ModifyTerrain(terrainEditor2D, transform.position, explosionRadius, DigFactor);
            ModifyTerrain(terrainEditor2D, transform.position - Vector3.right * explosionRadius * 1.5f, explosionRadius * 0.5f, -DigFactor * 0.5f);
            ModifyTerrain(terrainEditor2D, transform.position + Vector3.right * explosionRadius * 1.5f, explosionRadius * 0.5f, -DigFactor * 0.5f);
        }

        for (int i = 0; i < GameplayObjectManager.Instance.HealthComponentList.Count; ++i)
        {
            HealthComponent healtComponent = GameplayObjectManager.Instance.HealthComponentList[i];

            if (healtComponent == null)
            {
                continue;
            }

            CircleCollider2D circleCollider = healtComponent.gameObject.GetComponent<CircleCollider2D>();
            if(circleCollider != null)
            {
                Vector3 toCircleCollider = (healtComponent.transform.position - transform.position).normalized;
                if (circleCollider.OverlapPoint(transform.position + toCircleCollider * explosionRadius))
                {
                    healtComponent.Damage(Damage * (explosionRadius / ExplosionRadius));
                }
            }
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

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
}
