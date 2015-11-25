using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public GameObject referenceTracker;
    public float damping = 1.0f;
    void FixedUpdate ()
    {
        if (referenceTracker != null)
        {
            Vector3 tgtTransform = transform.position;
            tgtTransform.x = DampingUtility.Damp(tgtTransform.x, referenceTracker.transform.position.x, damping, TimeHelper.GameTime);
            transform.position = tgtTransform;
        }
    }
}
