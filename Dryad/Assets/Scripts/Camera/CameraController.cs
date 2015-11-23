using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public GameObject referenceTracker;
    public float cameraXDamping = 1.0f;
    private Vector3 myTransform;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        myTransform = this.transform.position;
        myTransform.x = DampingUtility.Damp(myTransform.x, referenceTracker.transform.position.x, cameraXDamping, TimeHelper.GameTime);
        this.transform.position = myTransform;
	}
}
