using UnityEngine;
using System.Collections;

public class Ballistic : MonoBehaviour
{

    public enum BallisticMode
    {
        PassThrough,
        Force,
    }

    public BallisticMode mode;
    private Vector3 startLocation;
    public Vector3 focusLocation;
    public float force = 1.0f;

    private float a, h, v;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        startLocation = transform.position;
        focusLocation = GetMouseWorldPosition();

        switch(mode)
        {
            case BallisticMode.PassThrough:
                h = focusLocation.x - startLocation.x;
                v = focusLocation.y - startLocation.y;
                a = (startLocation.y - v) / Mathf.Pow(startLocation.x - h, 2.0f);
                break;
            case BallisticMode.Force:
                h = startLocation.x + ((focusLocation - startLocation).normalized * force).x;
                v = startLocation.y + ((focusLocation - startLocation).normalized * force).y;
                a = (startLocation.y - v) / Mathf.Pow(startLocation.x - h, 2.0f);
                break;
        }

        force = Mathf.Max(0.1f, force + Input.mouseScrollDelta.y * 0.1f);
    }

    private float GetY(float X)
    {
        return a * Mathf.Pow(X - h, 2.0f) + v;
    }

    private Vector3 GetMouseWorldPosition()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        (new Plane(-Camera.main.transform.forward, 0.0f)).Raycast(ray, out distance);

        Vector3 position = ray.GetPoint(distance);

        return position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startLocation, 1.0f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(focusLocation, 1.0f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(startLocation, focusLocation);

        const int Rez = 36;
        const float Distance = 10.0f;
        Vector2 lastLocation = new Vector2(transform.position.x, GetY(transform.position.x));
        for(int i = 0; i < Rez; ++i)
        {
            float X = ((float)i / (float)(Rez-1)) * Distance + transform.position.x;
            float Y = GetY(X);

            Vector3 newPos = transform.position + new Vector3(X, Y);
            Gizmos.DrawLine(lastLocation, newPos);
            lastLocation = newPos;
        }
    }
}
