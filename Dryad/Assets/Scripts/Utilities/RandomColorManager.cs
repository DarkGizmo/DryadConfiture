using System.Collections.Generic;
using UnityEngine;

public class RandomColorManager
{
    private static readonly RandomColorManager m_Instance = new RandomColorManager();

    public static RandomColorManager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    private readonly Color[] COLOR_LIST = 
    {
        new Color(255.0f/255.0f, 145.0f/255.0f, 128.0f/255.0f), 
        new Color(255.0f/255.0f, 68.0f/255.0f, 0.0f/255.0f), 
        new Color(115.0f/255.0f, 94.0f/255.0f, 86.0f/255.0f), 
        new Color(255.0f/255.0f, 140.0f/255.0f, 64.0f/255.0f), 
        new Color(230.0f/255.0f, 203.0f/255.0f, 172.0f/255.0f), 
        new Color(153.0f/255.0f, 102.0f/255.0f, 0.0f/255.0f), 
        new Color(255.0f/255.0f, 204.0f/255.0f, 0.0f/255.0f), 
        new Color(102.0f/255.0f, 97.0f/255.0f, 77.0f/255.0f), 
        new Color(238.0f/255.0f, 255.0f/255.0f, 0.0f/255.0f), 
        new Color(173.0f/255.0f, 179.0f/255.0f, 89.0f/255.0f), 
        new Color(69.0f/255.0f, 153.0f/255.0f, 38.0f/255.0f), 
        new Color(34.0f/255.0f, 255.0f/255.0f, 0.0f/255.0f), 
        new Color(255.0f/255.0f, 255.0f/255.0f, 255.0f/255.0f), 
        new Color(182.0f/255.0f, 242.0f/255.0f, 190.0f/255.0f), 
        new Color(0.0f/255.0f, 255.0f/255.0f, 170.0f/255.0f), 
        new Color(32.0f/255.0f, 128.0f/255.0f, 108.0f/255.0f), 
        new Color(0.0f/255.0f, 255.0f/255.0f, 238.0f/255.0f), 
        new Color(134.0f/255.0f, 176.0f/255.0f, 179.0f/255.0f), 
        new Color(0.0f/255.0f, 92.0f/255.0f, 115.0f/255.0f), 
        new Color(115.0f/255.0f, 176.0f/255.0f, 230.0f/255.0f), 
        new Color(0.0f/255.0f, 34.0f/255.0f, 128.0f/255.0f), 
        new Color(115.0f/255.0f, 130.0f/255.0f, 230.0f/255.0f), 
        new Color(89.0f/255.0f, 64.0f/255.0f, 255.0f/255.0f), 
        new Color(101.0f/255.0f, 86.0f/255.0f, 115.0f/255.0f), 
        new Color(230.0f/255.0f, 182.0f/255.0f, 242.0f/255.0f), 
        new Color(255.0f/255.0f, 64.0f/255.0f, 242.0f/255.0f), 
        new Color(140.0f/255.0f, 0.0f/255.0f, 112.0f/255.0f), 
        new Color(255.0f/255.0f, 0.0f/255.0f, 102.0f/255.0f), 
        new Color(230.0f/255.0f, 115.0f/255.0f, 161.0f/255.0f), 
        new Color(140.0f/255.0f, 0.0f/255.0f, 19.0f/255.0f), 
        new Color(242.0f/255.0f, 182.0f/255.0f, 190.0f/255.0f)
    };

    private ulong m_CurrentId = 0;

    private Dictionary<ulong, List<Color>> m_AvailableColors = new Dictionary<ulong, List<Color>>();

    public ulong CreateColorGroup()
    {
        ulong colorGroup = m_CurrentId++;

        m_AvailableColors.Add(colorGroup, new List<Color>(COLOR_LIST));

        return colorGroup;
    }

    public Color GetUniqueColor(ulong groupId)
    {
        if (m_AvailableColors [groupId].Count > 0)
        {
            int randomIndex = Random.Range(0, m_AvailableColors [groupId].Count);
            Color randomColor = m_AvailableColors[groupId][randomIndex];
            m_AvailableColors[groupId].RemoveAt(randomIndex);

            return randomColor;
        }

        return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
    }

    public void PutBackColor(ulong groupId, Color color)
    {
        m_AvailableColors[groupId].Add(color);
    }
}
