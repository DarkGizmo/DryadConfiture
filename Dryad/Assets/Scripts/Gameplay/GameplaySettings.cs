using UnityEngine;

class GameplaySettings : MonoBehaviour
{
    private static GameplaySettings instance = null;
    public static GameplaySettings Instance { get { return instance; } }
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    
}
