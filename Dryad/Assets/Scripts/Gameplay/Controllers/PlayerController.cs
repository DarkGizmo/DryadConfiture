using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController
    : GroundController
    , PostRendererListenerInterface
{
    public enum EAimingSmoothType
    {
        Linear,
        ECurve,
        SCurve,
    }
    
    [Header("Player")]
    public float InputForce = 10.0f;
    public float flapIntensity = 5.0f;
    private bool mIsFloating = true;

    public override float GetVelocitySide()
    {
        if (!mIsAnchored && IsGrounded())
        {
            float inputForce = InputForce * Input.GetAxis("Horizontal");

            float side = Mathf.Sign(mGroundNormal.x) * -Mathf.Sign(inputForce);

            float groundAngle = Mathf.Rad2Deg * Mathf.Acos(mGroundNormal.y) * side;

            float speedScalingRatio = SlopeSpeed.Evaluate(Mathf.Clamp(groundAngle, 0.0f, 90.0f));

            return inputForce * speedScalingRatio;
        }

        return 0.0f;
    }

    private void UpdateFree(ref Vector2 newVelocity)
    {
        if (!mUnderground && Input.GetButtonDown("Anchor"))
        {
            newVelocity = new Vector2(newVelocity.x / 1.375f, -3.0f);
            mIsFloating = !mIsFloating;
            this.GetComponent<SpriteRenderer>().color = Color.white;
            mIsAnchored = false;
        }

        if(mUnderground)
        {
            mIsFloating = false;
        }

        if (mIsFloating)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (curHydro >= Mathf.Abs(hAdjFlap))
                {
                    newVelocity.y += flapIntensity;
                    SetIsGrounded(false);
                    adjHydro(hAdjFlap);
                }
            }
            if (curHydro >= Mathf.Abs(hAdjFree))
            {
                adjHydro(Mathf.Abs(Input.GetAxis("Horizontal")) * hAdjMove + hAdjFree);

                float inputForce = InputForce * TimeHelper.GameTime * Input.GetAxis("Horizontal");
                newVelocity += GetRight() * inputForce;
            }
        }
    }

    // Hydrometer
    public float maxHydro = 30.0f;
    private float curHydro;
    private float hBarLength;
    private float hAdjFree = -0.01f;
    private float hAdjMove = -0.1f;
    private float hAdjFlap = -3.0f;
    private float hAdjRefill = 0.3f;
    
    private void UpdateColor()
    {
        if(mUnderground)
        {
            this.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if (mIsAnchored)
        {
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if(IsGrounded())
        {
            this.GetComponent<SpriteRenderer>().color = ColorExtension.brown;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    // Anchored
    [Header("Ballistic")]
    public GameObject BallisticAmmo;
    public GameObject UndergroundBallisticAmmo;
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
        if(mUnderground)
        {
            mIsAnchored = true;
        }

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
                        GameObject ballistic = (GameObject)GameObject.Instantiate((mUnderground ? UndergroundBallisticAmmo : BallisticAmmo), transform.position, Quaternion.identity);
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
                    mIsFloating = false;
                }
            }
        }
    }

    // Use this for initialization
    public override void Start ()
    {
        base.Start();

        Camera.main.GetComponent<PostRendererEmitter>().RegisterPostRendererListener(this);
        hBarLength = Screen.width / 2;
        curHydro = maxHydro;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        UpdateColor();

        Vector2 newVelocity = GetComponent<Rigidbody2D>().velocity;

        UpdateFree(ref newVelocity);
        if (mIsFloating)
        {
            GetComponent<Rigidbody2D>().velocity = newVelocity;
        }

        UpdateAnchored();
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

    public new void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (isAiming)
        {
            Vector3 originalWorldAimingPosition = ScreenToWorldPoint(originalAimingPosition);
            Vector3 aimingWorldPosition = originalWorldAimingPosition + GetLaunchVector();

            float dist = GetLaunchForce();

            DebugExtension.DrawCircle(originalWorldAimingPosition, Vector3.forward, Mathf.Min(GetLaunchForce(), MaximumLaunchLength));
            DebugExtension.DrawCircle(originalWorldAimingPosition, Vector3.forward, MinimumLaunchLength);
            DebugExtension.DrawCircle(originalWorldAimingPosition, Vector3.forward, MaximumLaunchLength);
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
