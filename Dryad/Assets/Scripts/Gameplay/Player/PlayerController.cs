using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController
    : MonoBehaviour
    , PostRendererListenerInterface
{
    public enum EAimingSmoothType
    {
        Linear,
        ECurve,
        SCurve,
    }
    
    public float force = 30.0f;
    public float flapIntensity = 5.0f;
    public bool mIsFloating = true;
    public float maxHorizVelocity = 12.0f;
    public float MaxClimbAngle = 50.0f;
    public float ClimbSlowdown = 15.0f;
    public float SlopeSlowdownSpeed = 0.5f;

    private void UpdateFree(ref Vector2 newVelocity)
    {
        if (Input.GetButtonDown("Anchor"))
        {
            newVelocity = new Vector2(newVelocity.x / 1.375f, -3.0f);
            mIsFloating = !mIsFloating;
            this.GetComponent<SpriteRenderer>().color = Color.white;
            mIsAnchored = false;
        }

        if (mIsFloating)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (curHydro >= Mathf.Abs(hAdjFlap))
                {
                    newVelocity.y += flapIntensity;
                    mGrounded = false;
                    adjHydro(hAdjFlap);
                }
            }
            if (curHydro >= Mathf.Abs(hAdjFree))
            {
                adjHydro(Mathf.Abs(Input.GetAxis("Horizontal")) * hAdjMove + hAdjFree);

                float inputForce = force * TimeHelper.GameTime * Input.GetAxis("Horizontal");

                if(mGrounded)
                {
                    float side = Mathf.Sign(mGroundNormal.x) * -Mathf.Sign(inputForce);

                    float groundAngle = Mathf.Rad2Deg * Mathf.Acos(mGroundNormal.y) * side;

                    float inputSlowdownRatio = MathUtility.MapClamped(groundAngle, MaxClimbAngle - ClimbSlowdown, MaxClimbAngle, 1.0f, 0.0f);

                    inputForce *= inputSlowdownRatio;
                }

                newVelocity += GetRight() * inputForce;

                if (mGrounded)
                {
                    newVelocity = newVelocity.normalized * Mathf.Clamp(newVelocity.magnitude, -maxHorizVelocity, maxHorizVelocity);
                }
            }
        }
    }

    // Hydrometer
    public float maxHydro = 30.0f;
    public float curHydro;
    private float hBarLength;
    public float hAdjFree = -0.01f;
    public float hAdjMove = -0.1f;
    public float hAdjFlap = -3.0f;
    public float hAdjRefill = 0.3f;
    
    // Grounded
    bool mGrounded = false;
    Vector2 mGroundNormal;

    private void UpdateGrounded()
    {
        UpdateGroundNormal();

        if(mIsFloating)
        {
            if(mGrounded)
            {
                this.GetComponent<SpriteRenderer>().color = ColorExtension.brown;
            }
            else
            {
                this.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }

        if(mGrounded)
        {
            float angle = Mathf.Rad2Deg * (Mathf.Acos(mGroundNormal.y) * -Mathf.Sign(mGroundNormal.x));
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);

            if(angle > MaxClimbAngle)
            {
                Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
                GetComponent<Rigidbody2D>().velocity -= velocity.normalized * Mathf.Min(SlopeSlowdownSpeed * TimeHelper.GameTime, velocity.magnitude);
            }
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    // Anchored
    public GameObject BallisticAmmo;
    public float BallisticLaunchForce = 1.0f;
    public float BallisticFactor = 0.3f;
    public EAimingSmoothType AimingSmoothType;
    public float MaximumLaunchLength = 10.0f;
    public float MinimumLaunchLength = 1.0f;
    public Material ArrowMaterial;

    private bool mIsAnchored = false;
    private bool isAiming;
    private Vector3 originalAimingPosition;
    private Vector3 currentAimingPosition;

    private void UpdateAnchored()
    {
        if (mIsAnchored)
        {
            adjHydro(hAdjRefill);
            if (!isAiming && Input.GetButtonDown("Fire"))
            {
                originalAimingPosition = GetMouseScreenPosition();
                currentAimingPosition = GetMouseScreenPosition();
                isAiming = true;
            }
            else if (isAiming)
            {
                if (Input.GetButtonUp("Fire"))
                {
                    isAiming = false;

                    float launchForce = GetLaunchForce();
                    if (launchForce > 0.0f)
                    {
                        GameObject ballistic = (GameObject)GameObject.Instantiate(BallisticAmmo, transform.position, Quaternion.identity);
                        ballistic.GetComponent<BallisticAmmo>().Initialize(gameObject);
                        Vector3 launchVelocity = GetLaunchVector().normalized * GetShootingRatio() * BallisticLaunchForce;
                        ballistic.GetComponent<Rigidbody2D>().AddForce(launchVelocity);
                    }
                }
                else
                {
                    currentAimingPosition = GetMouseScreenPosition();
                }
            }
        }
        else
        {
            if (!mIsFloating)
            {
                if (GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                {
                    mIsAnchored = true;
                    this.GetComponent<SpriteRenderer>().color = Color.red;
                    mIsFloating = false;
                }
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        Camera.main.GetComponent<PostRendererEmitter>().RegisterPostRendererListener(this);
        hBarLength = Screen.width / 2;
        curHydro = maxHydro;
	}

    // Update is called once per frame
    void Update()
    {
        UpdateGrounded();

        Vector2 newVelocity = GetComponent<Rigidbody2D>().velocity;

        UpdateFree(ref newVelocity);
        
        GetComponent<Rigidbody2D>().velocity = newVelocity;

        UpdateAnchored();
    }

    private Vector2 GetGroundNormal()
    {
        return mGroundNormal;
    }

    private Vector2 GetRight()
    {
        if (!VectorUtility.IsZero(mGroundNormal))
        {
            Vector3 cross = Vector3.Cross(VectorUtility.ToVector3(GetGroundNormal()), Vector3.forward);
            return VectorUtility.ToVector2(cross);
        }

        return Vector2.right;
    }

    private Vector3 GetLaunchVector()
    {
        Vector3 rangeVector = ScreenToWorldPoint(originalAimingPosition) - ScreenToWorldPoint(currentAimingPosition);
        return rangeVector;
    }

    private float GetLaunchForce()
    {
        float launchForce = GetLaunchVector().magnitude;
        if (launchForce > MinimumLaunchLength)
        {
            return Mathf.Min(launchForce, MaximumLaunchLength);
        }

        return 0.0f;
    }

    private float GetShootingValue(float shootingMagnitude)
    {
        float factor = 0.0f;

        switch(AimingSmoothType)
        {
            case EAimingSmoothType.Linear:
                factor = shootingMagnitude / MaximumLaunchLength;
                break;
            case EAimingSmoothType.ECurve:
                factor = 1.0f - Mathf.Exp(-(shootingMagnitude) / BallisticFactor);
                break;
            case EAimingSmoothType.SCurve:
                {
                    float x = shootingMagnitude / MaximumLaunchLength * 0.5f + 0.5f;
                    factor = x * x * x * (x * (x * 6.0f - 15.0f) + 10.0f);
                }
                break;
        }

        return Mathf.Clamp01(factor);
    }

    private float GetShootingRatio()
    {
        return GetShootingValue(GetLaunchVector().magnitude);
    }

    private Vector3 GetMouseWorldPosition()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        (new Plane(-Camera.main.transform.forward, 0.0f)).Raycast(ray, out distance);

        Vector3 position = ray.GetPoint(distance);

        return position;
    }

    void UpdateGroundNormal()
    {
        Collider2D collider = GetComponent<Collider2D>();
        float halfWidth = collider.bounds.extents.x;
        float halfHeight = collider.bounds.extents.y;

        int layerMask = 1 << LayerMask.NameToLayer("Terrain");
        float downCheckLength = 0.025f;
        RaycastHit2D rightHit = PhysicsHelper.Physics2DRaycast(VectorUtility.ToVector2(transform.position) + Vector2.right * halfWidth + Vector2.up * halfHeight, Vector2.down, halfHeight * 2.0f + downCheckLength, layerMask);
        RaycastHit2D centerHit = PhysicsHelper.Physics2DRaycast(VectorUtility.ToVector2(transform.position), Vector2.down, halfHeight + downCheckLength, layerMask);
        RaycastHit2D leftHit = PhysicsHelper.Physics2DRaycast(VectorUtility.ToVector2(transform.position) - Vector2.right * halfWidth + Vector2.up * halfHeight, Vector2.down, halfHeight * 2.0f + downCheckLength, layerMask);

        float input = Input.GetAxis("Horizontal");

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

    public float x, y;

    public Vector3 WorldToScreenPoint(Vector3 world)
    {
        Vector3 screenPoint = world - Camera.main.transform.position;
        screenPoint.Set(screenPoint.x / Camera.main.orthographicSize * 0.5f / Camera.main.aspect + 0.5f, screenPoint.y / Camera.main.orthographicSize * 0.5f + 0.5f, 0.0f);
        return screenPoint;
    }

    public Vector3 ScreenToWorldPoint(Vector3 screenPoint)
    {
        Vector3 worldPoint = new Vector3((screenPoint.x - 0.5f) * (Camera.main.orthographicSize * 2.0f * Camera.main.aspect), (screenPoint.y - 0.5f) * (Camera.main.orthographicSize * 2.0f));
        worldPoint.Set(worldPoint.x + Camera.main.transform.position.x, worldPoint.y + Camera.main.transform.position.y, 0.0f);

        return worldPoint;
    }

    public Vector3 GetMouseScreenPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.Set(mouseScreenPosition.x / Screen.width, mouseScreenPosition.y / Screen.height, 0.0f);

        return mouseScreenPosition;
    }

    public void OnPostRender()
    {
        GL.PushMatrix();
        GL.LoadOrtho();

        if (isAiming)
        {
            float launchForce = GetLaunchForce();
            if (launchForce > 0.0f)
            {
                Vector3 originalWorldAimingPosition = WorldToScreenPoint(transform.position);
                Vector3 aimingWorldPosition = transform.position + GetLaunchVector().normalized * GetShootingRatio() * MaximumLaunchLength;

                LineUtility.DrawLine(originalWorldAimingPosition, WorldToScreenPoint(aimingWorldPosition), ArrowMaterial);
            }
        }

        GL.PopMatrix();
    }

    public void OnDrawGizmos()
    {
        if (isAiming)
        {
            Vector3 originalWorldAimingPosition = ScreenToWorldPoint(originalAimingPosition);
            Vector3 aimingWorldPosition = originalWorldAimingPosition + GetLaunchVector();

            float dist = GetLaunchForce();

            DebugExtension.DrawCircle(originalWorldAimingPosition, Vector3.forward, Mathf.Min(GetLaunchForce(), MaximumLaunchLength));
            DebugExtension.DrawCircle(originalWorldAimingPosition, Vector3.forward, MinimumLaunchLength);
            DebugExtension.DrawCircle(originalWorldAimingPosition, Vector3.forward, MaximumLaunchLength);
        }

        if (mGrounded)
        {
            Gizmos.DrawLine(transform.position, transform.position + VectorUtility.ToVector3(GetGroundNormal() * 2.0f));
            Gizmos.DrawLine(transform.position, transform.position + VectorUtility.ToVector3(GetRight() * 2.0f));
        }

        GizmosExtension.DrawDelayedGizmos();
    }
    public void adjHydro (float adj)
    {
        curHydro = Mathf.Clamp(curHydro+=adj*(TimeHelper.GameTime*30), 0.0f,maxHydro);
    }
    public void OnGUI()
    {
        GUI.HorizontalSlider(new Rect(new Vector2(hBarLength/2, Screen.height-20.0f), new Vector2 (hBarLength,1.0f)),curHydro,0.0f,maxHydro);
    }

    #region Cheats
    public void CheatRefillHydro()
    {
        curHydro = maxHydro;
    }
    #endregion
}
