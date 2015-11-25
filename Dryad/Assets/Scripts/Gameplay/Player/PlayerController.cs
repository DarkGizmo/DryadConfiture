using UnityEngine;
using System.Collections;

public class PlayerController
    : MonoBehaviour
    , PostRendererListenerInterface
{
    public float force = 30.0f;
    public float flapIntensity = 5.0f;
    public bool stateFree = true;
    public float maxHorizVelocity = 12.0f;
    private bool isAnchored = false;

    public GameObject BallisticAmmo;
    public float BallisticLaunchForce = 1.0f;
    public float BallisticFactor = 0.3f;

    public Material ArrowMaterial;

    private bool isAiming;
    private Vector3 originalAimingPosition;
    private Vector3 currentAimingPosition;

    // Use this for initialization
    void Start ()
    {
        Camera.main.GetComponent<PostRendererEmitter>().RegisterPostRendererListener(this);
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
                myVelocity.y+=flapIntensity;
            }
            myVelocity.x= Mathf.Clamp(myVelocity.x+force * TimeHelper.GameTime * Input.GetAxis("Horizontal"),-maxHorizVelocity,maxHorizVelocity);
        } else
        {
            if (!isAnchored && myVelocity == Vector2.zero)
            {
                isAnchored = true;
                this.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        GetComponent<Rigidbody2D>().velocity = myVelocity;

        if (isAnchored)
        {
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

                    GameObject ballistic = (GameObject)GameObject.Instantiate(BallisticAmmo, transform.position, Quaternion.identity);

                    Vector3 rangeVector = ScreenToWorldPoint(originalAimingPosition) - ScreenToWorldPoint(currentAimingPosition);

                    ballistic.GetComponent<Rigidbody2D>().AddForce(GetShootingRatio() * BallisticLaunchForce  * rangeVector.normalized);
                }
                else
                {
                    currentAimingPosition = GetMouseScreenPosition();
                }
            }
        }
    }

    private float GetShootingRatio()
    {
        //1 - e ^ (-x / .3)
        Vector3 rangeVector = ScreenToWorldPoint(originalAimingPosition) - ScreenToWorldPoint(currentAimingPosition);

        float factor = 1.0f - Mathf.Exp(-rangeVector.magnitude / BallisticFactor);

        return factor;
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
            LineUtility.DrawLine(originalAimingPosition, originalAimingPosition + originalAimingPosition - currentAimingPosition, ArrowMaterial);
        }

        GL.PopMatrix();
    }

    public void OnDrawGizmos()
    {
        if (isAiming)
        {
            Gizmos.DrawWireSphere(ScreenToWorldPoint(originalAimingPosition), 0.5f);
            Gizmos.DrawWireSphere(ScreenToWorldPoint(currentAimingPosition), 0.5f);

            Vector3 cubeSize = new Vector3(0.5f, 5.0f, 1.0f);
            Gizmos.DrawWireCube(ScreenToWorldPoint(GetMouseScreenPosition()) + Vector3.up * cubeSize.y * 0.5f, cubeSize);
            Vector3 fillSize = cubeSize - Vector3.up * cubeSize.y * (1.0f - GetShootingRatio());
            Gizmos.DrawCube(ScreenToWorldPoint(GetMouseScreenPosition()) + Vector3.up * fillSize.y * 0.5f, fillSize);
        }

        
    }
}
