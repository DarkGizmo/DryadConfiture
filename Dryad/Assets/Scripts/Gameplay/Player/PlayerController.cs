using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public float force = 30.0f;
    public float flapIntensity = 5.0f;
    public bool stateFree = true;
    public float maxHorizVelocity = 12.0f;
    private bool isAnchored = false;

    public GameObject BallisticAmmo;
    private float BallisticLaunchForce;
    public float LaunchForcePerSecond = 5.0f;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        Vector2 myVelocity = GetComponent<Rigidbody2D>().velocity;
        if (Input.GetButtonDown("Anchor"))
        {
            myVelocity = new Vector2 (myVelocity.x/1.375f, -3.0f);
            stateFree = !stateFree;
            this.GetComponent<SpriteRenderer>().color = Color.white;
            isAnchored = false;
        }
        if (stateFree)
        {
            if (Input.GetButtonDown("Jump"))
            {
                myVelocity.y+=flapIntensity;
            }
            myVelocity.x= Mathf.Clamp(myVelocity.x+force * TimeHelper.GameTime * Input.GetAxis("Horizontal"),-maxHorizVelocity,maxHorizVelocity);
        } else
        {
            if (!isAnchored && myVelocity == Vector2.zero)
            {
                isAnchored = true;
                this.GetComponent<SpriteRenderer>().color = Color.red;
            }

            if (isAnchored)
            {
                if(Input.GetButton("Fire"))
                {
                    BallisticLaunchForce += TimeHelper.GameTime * LaunchForcePerSecond;
                    
                }
                else if (BallisticLaunchForce > 0.0f)
                {
                    GameObject ballistic = (GameObject)GameObject.Instantiate(BallisticAmmo, transform.position, Quaternion.identity);
                    ballistic.GetComponent<Rigidbody2D>().AddForce((GetMouseWorldPosition() - transform.position).normalized * BallisticLaunchForce);

                    BallisticLaunchForce = 0.0f;
                }
            }
        }
        GetComponent<Rigidbody2D>().velocity = myVelocity;
    }

    private Vector3 GetMouseWorldPosition()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        (new Plane(-Camera.main.transform.forward, 0.0f)).Raycast(ray, out distance);

        Vector3 position = ray.GetPoint(distance);

        return position;
    }
}
