using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public GameObject referenceTracker;
    public float Damping = 1.0f;
    public float MaxLookahead = 30.0f;
    public float Lookahead;
    public float LookaheadDamping = 0.1f;
    public float LookaheadSpeed = 0.1f;
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    void FixedUpdate ()
    {
        Lookahead = DampingUtility.Damp(Lookahead, Mathf.Clamp(Lookahead+Input.GetAxis("Horizontal") * LookaheadSpeed, -MaxLookahead, MaxLookahead),LookaheadDamping,TimeHelper.GameTime);
        if (referenceTracker != null)
        {
            Vector3 tgtTransform = transform.position;
            tgtTransform.x = DampingUtility.Damp(tgtTransform.x, referenceTracker.transform.position.x+Lookahead, Damping, TimeHelper.GameTime);
            transform.position = tgtTransform;
        }
    }
}