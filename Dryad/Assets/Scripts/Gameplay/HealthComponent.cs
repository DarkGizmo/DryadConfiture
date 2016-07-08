using UnityEngine;
using System.Collections;

public class HealthComponent : MonoBehaviour
{
    public float Health = 100.0f;
	
    public void Awake()
    {
        GameplayObjectManager.Instance.RegisterBehaviour(this);
    }

    public void Damage(float amount)
    {
        Health = Mathf.Max(Health - amount, 0.0f);

        if(Health == 0.0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Health = 0.0f;
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        GameplayObjectManager.Instance.UnregisterBehaviour(this);
    }
}
