using UnityEngine;
using System.Collections.Generic;

public class FadeOutManager : MonoBehaviour, GUIInterface
{
    public delegate void FadeDoneCallBack();

    private static FadeOutManager instance = null;
    public static FadeOutManager Instance { get { return instance; } }
    
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

    public Texture2D m_FadeOutTexture;
    public Color m_FadeOutColor;
    public float m_FadeSpeed = 1.0f;

    public bool m_StartsFadedOut = false;
    private bool m_FadeOut = false;
    private float m_FadeOutRatio = 0.0f;
    private FadeDoneCallBack m_Callback = null;

    public void Start()
    {
        m_FadeOutRatio = m_StartsFadedOut ? 1.0f : 0.0f;
        GUIManager.RegisterGUIObject(this);
    }

    public void OnDestroy()
    {
        GUIManager.UnregisterGUIObject(this);
    }

    public static bool IsFullyFadedIn()
    {
        return FadeOutManager.Instance == null || FadeOutManager.Instance.m_FadeOutRatio == 0.0f;
    }

    private float GetSmoothRatio()
    {
        return DampingUtility.SinSmooth(m_FadeOutRatio);
    }

    public void FadeOut(float duration, FadeDoneCallBack callback)
    {
        m_FadeSpeed = 1.0f / duration;
        m_FadeOut = true;
        m_Callback = callback;
    }

    public void FadeIn(float duration, FadeDoneCallBack callback)
    {
        m_FadeSpeed = 1.0f / duration;
        m_FadeOut = false;
        m_Callback = callback;
    }

    public void Update()
    {
        m_FadeOutRatio = Mathf.Clamp01(m_FadeOutRatio + TimeHelper.GameTime * m_FadeSpeed * (m_FadeOut ? 1.0f : -1.0f));

        if (m_Callback != null)
        {
            if (m_FadeOutRatio == 1.0f && m_FadeOut)
            {
                if (m_Callback != null)
                {
                    m_Callback();
                    m_Callback = null;
                }
            }
            else if (m_FadeOutRatio == 0.0f && !m_FadeOut)
            {
                if (m_Callback != null)
                {
                    m_Callback();
                    m_Callback = null;
                }
            }
        }
    }

    #region GUIInterface implementation

    public int GetOrder()
    {
        return int.MaxValue;
    }

    public void OnOrderedGUI()
    {
        if (m_FadeOutRatio > 0.0f)
        {
            GUIUtility.PushColor();
            {
                GUI.color = new Color(m_FadeOutColor.r, m_FadeOutColor.g, m_FadeOutColor.b, GetSmoothRatio());
                GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), m_FadeOutTexture);
            }
            GUIUtility.PopColor();
        }
    }

    #endregion
}
