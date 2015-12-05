using UnityEngine;
using System.Collections;

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
    public bool stateFree = true;
    public float maxHorizVelocity = 12.0f;
    private bool isAnchored = false;

    public GameObject BallisticAmmo;
    public float BallisticLaunchForce = 1.0f;
    public float BallisticFactor = 0.3f;
    public EAimingSmoothType AimingSmoothType;

    public float MaximumLaunchLength = 10.0f;
    public float MinimumLaunchLength = 1.0f;
    public float MinimumLaunchForce = 1.0f;// still used?

    public Material ArrowMaterial;

    private bool isAiming;
    private Vector3 originalAimingPosition;
    private Vector3 currentAimingPosition;
    // Hydrometer
    public float maxHydro = 30.0f;
    public float curHydro;
    private float hBarLength;
    public float hAdjFree = -0.01f;
    public float hAdjMove = -0.1f;
    public float hAdjFlap = -3.0f;
    public float hAdjRefill = 0.3f;
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
                if (curHydro >= Mathf.Abs(hAdjFlap))
                {
                    myVelocity.y += flapIntensity;
                    adjHydro(hAdjFlap);
                }
            }
            if (curHydro >= Mathf.Abs(hAdjFree))
            {
                adjHydro(Mathf.Abs(Input.GetAxis("Horizontal")) * hAdjMove + hAdjFree);
                myVelocity.x = Mathf.Clamp(myVelocity.x + force * TimeHelper.GameTime * Input.GetAxis("Horizontal"), -maxHorizVelocity, maxHorizVelocity);
            }
        } else
        {
            if (!isAnchored && myVelocity == Vector2.zero)
            {
                isAnchored = true;
                this.GetComponent<SpriteRenderer>().color = Color.red;
                stateFree = false;
            }
        }
        GetComponent<Rigidbody2D>().velocity = myVelocity;

        if (isAnchored)
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

                    ballistic.GetComponent<BallisticAmmo>().Initialize(gameObject);
                    float launchForce = GetLaunchForce();
                    if (launchForce > 0.0f)
                    {
                        GameObject ballistic = (GameObject)GameObject.Instantiate(BallisticAmmo, transform.position, Quaternion.identity);
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
                Vector3 originalWorldAimingPosition = ScreenToWorldPoint(originalAimingPosition);
                Vector3 currentWorldAimingPosition = ScreenToWorldPoint(originalAimingPosition);
                Vector3 aimingWorldPosition = originalWorldAimingPosition + GetLaunchVector().normalized * GetShootingRatio() * MaximumLaunchLength;

                LineUtility.DrawLine(originalAimingPosition, WorldToScreenPoint(aimingWorldPosition), ArrowMaterial);
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
    }
    public void adjHydro (float adj)
    {
        curHydro = Mathf.Clamp(curHydro+=adj*(TimeHelper.GameTime*30), 0.0f,maxHydro);
    }
    public void OnGUI()
    {
        GUI.HorizontalSlider(new Rect(new Vector2(hBarLength/2, Screen.height-20.0f), new Vector2 (hBarLength,1.0f)),curHydro,0.0f,maxHydro);
    }
}
