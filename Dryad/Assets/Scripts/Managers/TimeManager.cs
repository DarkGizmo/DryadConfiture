using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TimeType
{
    Engine, // Includes physics, animation, any system not user defined
    Camera, // Includes any code defined by the user relating to camera
    Gameplay, // Includes any code defined by the user relating to anything else
}

public class TimeManager
{
    private static float[] m_DesiredTimeDilatations = new float[] { 1.0f, 1.0f, 1.0f };
    private static float[] m_ActualTimeDilatations = new float[] { 1.0f, 1.0f, 1.0f };

    static public void SetTimeDilatation(TimeType type, float dilatation)
    {
        m_DesiredTimeDilatations[(int)type] = dilatation;

        UpdateDilatations();
    }

    static public float GetTimeDilatation(TimeType type)
    {
        return m_DesiredTimeDilatations[(int)type];
    }

    static public float GetTime(TimeType type)
    {
        if(Time.timeScale != 0.0f)
        {
            return Time.deltaTime * m_ActualTimeDilatations[(int)type];
        }
        else
        {
            return Time.fixedDeltaTime * m_ActualTimeDilatations[(int)type];
        }
    }

    static private void UpdateDilatations()
    {
        Time.timeScale = m_ActualTimeDilatations[(int)TimeType.Engine] = m_DesiredTimeDilatations[(int)TimeType.Engine];
        
        // Skip engine
        for (int i = 1; i < m_ActualTimeDilatations.Length; ++i)
        {
            if(Time.timeScale != 0.0f)
            {
                m_ActualTimeDilatations[i] = m_DesiredTimeDilatations[i] / Time.timeScale;
            }
            else
            {
                m_ActualTimeDilatations[i] = m_DesiredTimeDilatations[i];
            }
        }
    }
}
