using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GroundController : MonoBehaviour
{
    public AnimationCurve SlopeSpeed = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
    public float MaxHorizVelocity = 12.0f;

    protected bool mGrounded;
    protected bool mUnderground;
    protected Vector2 mGroundNormal;
    protected Vector3 mGroundPosition;

    public abstract float GetVelocitySide();

    private TerrainEditor2D mGroundTerrain;

    public virtual void Start()
    {
        mGroundTerrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainEditor2D>();
    }

    public bool IsGrounded()
    {
        return mGrounded;
    }

    public void SetIsGrounded(bool value)
    {
        mGrounded = value;
    }

    private void UpdateController(float inputForce)
    {
        UpdateGroundPosition();
        UpdateGroundNormal(inputForce);

        if (mGrounded)
        {
            float angle = Mathf.Rad2Deg * (Mathf.Acos(mGroundNormal.y) * -Mathf.Sign(mGroundNormal.x));
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        }
    }

    public void Reset()
    {
        SlopeSpeed = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
    }

    public virtual void Update()
    {
        float inputForce = GetVelocitySide();

        UpdateController(inputForce);

        Vector2 newVelocity = GetComponent<Rigidbody2D>().velocity;

        if (mGrounded)
        {
            float side = Mathf.Sign(mGroundNormal.x) * -Mathf.Sign(inputForce);

            float groundAngle = Mathf.Rad2Deg * Mathf.Acos(mGroundNormal.y) * side;

            float speedScalingRatio = SlopeSpeed.Evaluate(Mathf.Clamp(groundAngle, 0.0f, 90.0f));

            inputForce *= speedScalingRatio;
        }

        newVelocity += GetRight() * inputForce;

        if (mGrounded)
        {
            newVelocity = newVelocity.normalized * Mathf.Clamp(newVelocity.magnitude, -MaxHorizVelocity, MaxHorizVelocity);
        }

        GetComponent<Rigidbody2D>().velocity = newVelocity;
    }

    protected Vector2 GetGroundNormal()
    {
        return mGroundNormal;
    }

    protected Vector2 GetRight()
    {
        if (!VectorUtility.IsZero(mGroundNormal))
        {
            Vector3 cross = Vector3.Cross(VectorUtility.ToVector3(GetGroundNormal()), Vector3.forward);
            return VectorUtility.ToVector2(cross);
        }

        return Vector2.right;
    }

    void UpdateGroundPosition()
    {
        if (mGroundTerrain == null)
        {
            mGroundTerrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainEditor2D>();
        }

        if (mGroundTerrain != null)
        {
            mGroundPosition = mGroundTerrain.GetGroundPosition(transform.position);
        }

        mUnderground = mGroundPosition.y > transform.position.y;

        if (mUnderground)
        {
            GetComponent<Rigidbody2D>().constraints |= RigidbodyConstraints2D.FreezePositionY;
        }
        else
        {
            GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        }
    }

    void UpdateGroundNormal(float inputForce)
    {
        Collider2D collider = GetComponent<Collider2D>();
        float halfWidth = collider.bounds.extents.x;
        float halfHeight = collider.bounds.extents.y;

        int layerMask = 1 << LayerMask.NameToLayer("Terrain");
        float downCheckLength = 0.025f;
        RaycastHit2D rightHit = PhysicsHelper.Physics2DRaycast(VectorUtility.ToVector2(transform.position) + Vector2.right * halfWidth + Vector2.up * halfHeight, Vector2.down, halfHeight * 2.0f + downCheckLength, layerMask);
        RaycastHit2D centerHit = PhysicsHelper.Physics2DRaycast(VectorUtility.ToVector2(transform.position), Vector2.down, halfHeight + downCheckLength, layerMask);
        RaycastHit2D leftHit = PhysicsHelper.Physics2DRaycast(VectorUtility.ToVector2(transform.position) - Vector2.right * halfWidth + Vector2.up * halfHeight, Vector2.down, halfHeight * 2.0f + downCheckLength, layerMask);

        float input = inputForce;

        int hitCount = 0;
        Vector2 hitPoint = Vector2.zero;
        mGrounded = false;
        mGroundNormal = Vector2.zero;
        if (rightHit.collider && input >= 0.0f)
        {
            ++hitCount;
            hitPoint += rightHit.point;
            mGroundNormal += rightHit.normal;
        }
        if (leftHit.collider && input <= 0.0f)
        {
            ++hitCount;
            hitPoint += leftHit.point;
            mGroundNormal += leftHit.normal;
        }
        if (centerHit.collider && ((input >= -0.3f && input < 0.3f) || VectorUtility.IsZero(mGroundNormal)))
        {
            ++hitCount;
            hitPoint += centerHit.point;
            mGroundNormal += centerHit.normal;
        }

        if (!VectorUtility.IsZero(mGroundNormal))
        {
            mGrounded = true;
            mGroundNormal.Normalize();
        }

        if(mGrounded)
        {
            float newY = (hitPoint.y / (float)hitCount) + halfHeight;
            if (newY < transform.position.y)
            {
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (mGrounded)
        {
            Gizmos.DrawLine(transform.position, transform.position + VectorUtility.ToVector3(GetGroundNormal() * 2.0f));
            Gizmos.DrawLine(transform.position, transform.position + VectorUtility.ToVector3(GetRight() * 2.0f));
        }


        Gizmos.DrawWireSphere(mGroundPosition, 0.25f);
    }
}
