using UnityEngine;
using System.Collections.Generic;

public class FullscreenImage : MonoBehaviour, GUIInterface
{
    public Texture2D m_Image;
    public Color m_BackgroundColor;
    
    public void Start()
    {
        Camera.main.backgroundColor = m_BackgroundColor;
        GUIManager.RegisterGUIObject(this);
    }

    public void OnDestroy()
    {
        GUIManager.UnregisterGUIObject(this);
    }

    #region GUIInterface implementation

    public int GetOrder()
    {
        return -100;
    }

    public void OnOrderedGUI()
    {
        if(m_Image != null)
        {
            float textureRatio = (float)m_Image.width / (float)m_Image.height;
            float screenRatio = (float)Screen.width / (float)Screen.height;
            
            float height = 0.0f;
            float width = 0.0f;
            if(screenRatio > textureRatio)
            {
                height = Screen.height;
                width = m_Image.width * (height / m_Image.height);
            }
            else
            {
                width = Screen.width;
                height = m_Image.height * (width / (float)m_Image.width);
            }
            
            float xOffset = (Screen.width - width) * 0.5f;
            float yOffset = (Screen.height - height) * 0.5f;
            
            GUI.DrawTexture(new Rect(xOffset, yOffset, width, height), m_Image);
        }
    }

    #endregion
}
