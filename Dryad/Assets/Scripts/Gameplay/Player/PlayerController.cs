using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public float force = 30.0f;
    public string btnFlap = "Fire1";
    public string btnAnchor = "Fire2";
    public float flapIntensity = 5.0f;
    public bool stateFree = true;
    public float maxHorizVelocity = 12.0f;
    private bool isAnchored = false;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        Vector2 myVelocity = GetComponent<Rigidbody2D>().velocity;
        if (Input.GetButtonDown(btnAnchor))
        {
            myVelocity = new Vector2 (myVelocity.x/1.375f, -3.0f);
            stateFree = !stateFree;
            this.GetComponent<SpriteRenderer>().color = Color.white;
            isAnchored = false;
        }
        if (stateFree)
        {
            if (Input.GetButtonDown(btnFlap))
            {
                myVelocity.y+=flapIntensity;
            }
            myVelocity.x= Mathf.Clamp(myVelocity.x+force * TimeHelper.GameTime * Input.GetAxis("Horizontal"),-maxHorizVelocity,maxHorizVelocity);
        } else
        {
            if (isAnchored)
            {
                this.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else if (myVelocity == Vector2.zero)
            {
                isAnchored = true;
            }
        }
        GetComponent<Rigidbody2D>().velocity = myVelocity;
    }
}
