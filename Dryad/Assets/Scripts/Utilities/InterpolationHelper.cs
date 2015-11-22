using UnityEngine;

public class InterpolationHelper
{
    public static Rect LerpRect(Rect A, Rect B, float ratio)
    {
        return 
            new Rect(
                Mathf.Lerp(A.position.x, B.position.x, ratio),
                Mathf.Lerp(A.position.y, B.position.y, ratio),
                Mathf.Lerp(A.width, B.width, ratio),
                Mathf.Lerp(A.height, B.height, ratio));

    }
}
