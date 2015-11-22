using System.Collections.Generic;
using UnityEngine;

class GUIUtility
{
    private static List<GUIStyle> m_LabelStyleStack = new List<GUIStyle>();
    private static List<Color> m_ColorStack = new List<Color>();

    public static void PushColor()
    {
        m_ColorStack.Add(GUI.color);
    }

    public static void PopColor()
    {
        GUI.color = m_ColorStack [m_ColorStack.Count - 1];
        m_ColorStack.RemoveAt(m_ColorStack.Count - 1);
    }

    public static void PushLabelStyle()
    {
        m_LabelStyleStack.Add(new GUIStyle(GUI.skin.label));
    }

    public static void PopLabelStyle()
    {
        GUI.skin.label = m_LabelStyleStack [m_LabelStyleStack.Count - 1];
        m_LabelStyleStack.RemoveAt(m_LabelStyleStack.Count - 1);
    }
}
