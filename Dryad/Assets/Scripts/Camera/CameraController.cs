using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public GameObject referenceTracker;
    public float cameraXDamping = 1.0f;
  
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 camPosition = this.transform.position;
        camPosition.x = DampingUtility.Damp(camPosition.x, referenceTracker.transform.position.x, cameraXDamping, TimeHelper.GameTime);
        this.transform.position = camPosition;
	}
}
