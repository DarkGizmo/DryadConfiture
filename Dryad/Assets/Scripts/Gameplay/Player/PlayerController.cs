using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public float force = 30.0f;
    public string btnFlap = "Fire1";
    public string btnAnchor = "Fire2";
    public float flapIntensity = 5.0f;
    public bool stateFree = true;
    public float maxHorizVelocity = 30.0f;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        Vector2 myVelocity = GetComponent<Rigidbody2D>().velocity;
        if (Input.GetButtonDown(btnAnchor))
        {
            myVelocity = myVelocity/2;
            stateFree = !stateFree;
        }
        if (stateFree)
        {
            if (Input.GetButtonDown(btnFlap))
            {
                myVelocity.y+=flapIntensity;
            }
            myVelocity.x= Mathf.Clamp(myVelocity.x+force * TimeHelper.GameTime * Input.GetAxis("Horizontal"),-maxHorizVelocity,maxHorizVelocity);
        }
        GetComponent<Rigidbody2D>().velocity = myVelocity;
    }
}
