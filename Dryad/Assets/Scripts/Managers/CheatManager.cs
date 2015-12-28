
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private static CheatManager instance = null;
    public static CheatManager Instance { get { return instance; } }
    
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

    public void Update()
    {
        bool timeDilatationUpdated = false;

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if(Input.GetKeyDown(KeyCode.Pause))
        {
            if(TimeManager.GetTimeDilatation(TimeType.Engine) != 0.0f)
            {
                TimeManager.SetTimeDilatation(TimeType.Gameplay, 0.0f);
                TimeManager.SetTimeDilatation(TimeType.Engine, 0.0f);
            }
            else
            {
                TimeManager.SetTimeDilatation(TimeType.Gameplay, 1.0f);
                TimeManager.SetTimeDilatation(TimeType.Engine, 1.0f);
            }
            timeDilatationUpdated = true;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                timeDilatationUpdated = true;
                TimeManager.SetTimeDilatation(TimeType.Engine, 0.25f);
                TimeManager.SetTimeDilatation(TimeType.Gameplay, 0.25f);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                timeDilatationUpdated = true;
                TimeManager.SetTimeDilatation(TimeType.Engine, 0.5f);
                TimeManager.SetTimeDilatation(TimeType.Gameplay, 0.5f);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                timeDilatationUpdated = true;
                TimeManager.SetTimeDilatation(TimeType.Engine, 1.0f);
                TimeManager.SetTimeDilatation(TimeType.Gameplay, 1.0f);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                timeDilatationUpdated = true;
                TimeManager.SetTimeDilatation(TimeType.Engine, 2.0f);
                TimeManager.SetTimeDilatation(TimeType.Gameplay, 2.0f);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha5))
            {
                timeDilatationUpdated = true;
                TimeManager.SetTimeDilatation(TimeType.Engine, 4.0f);
                TimeManager.SetTimeDilatation(TimeType.Gameplay, 4.0f);
            }
            else if(Input.GetKeyDown(KeyCode.PageUp))
            {
                timeDilatationUpdated = true;
                TimeManager.SetTimeDilatation(TimeType.Engine, TimeManager.GetTimeDilatation(TimeType.Engine) * 2.0f);
                TimeManager.SetTimeDilatation(TimeType.Gameplay, TimeManager.GetTimeDilatation(TimeType.Gameplay) * 2.0f);
            }
            else if(Input.GetKeyDown(KeyCode.PageDown))
            {
                timeDilatationUpdated = true;
                TimeManager.SetTimeDilatation(TimeType.Engine, TimeManager.GetTimeDilatation(TimeType.Engine) * 0.5f);
                TimeManager.SetTimeDilatation(TimeType.Gameplay, TimeManager.GetTimeDilatation(TimeType.Gameplay) * 0.5f);
            }
            else if(Input.GetKeyDown(KeyCode.R))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }

        if(Input.GetKeyDown(KeyCode.Home))
        {
            PlayerController pc = GameObject.FindObjectOfType<PlayerController>();
            pc.CheatRefillHydro();
        }

        if(timeDilatationUpdated)
        {
            Debug.Log("Time Dilatation: " + TimeManager.GetTimeDilatation(TimeType.Gameplay));
        }
    }
}
