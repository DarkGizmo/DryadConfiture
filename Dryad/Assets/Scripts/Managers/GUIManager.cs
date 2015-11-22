
using UnityEngine;
using System.Collections.Generic;

public interface GUIInterface
{
    int GetOrder();
    void OnOrderedGUI();

}

public class GUIManager : MonoBehaviour
{
    private static GUIManager instance = null;
    public static GUIManager Instance { get { return instance; } }
    
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

    private SortedList<int, List<GUIInterface>> m_GUIObjects = new SortedList<int, List<GUIInterface>>();

    public static void RegisterGUIObject(GUIInterface guiObject)
    {
        if (GUIManager.Instance != null)
        {
            GUIManager.Instance.InternalRegisterGUIObject(guiObject);
        }
    }

    private void InternalRegisterGUIObject(GUIInterface guiObject)
    {
        List<GUIInterface> list;
        if (!m_GUIObjects.TryGetValue(guiObject.GetOrder(), out list))
        {
            list = new List<GUIInterface>();
            m_GUIObjects.Add(guiObject.GetOrder(), list);
        }

        list.Add(guiObject);
    }

    public static void UnregisterGUIObject(GUIInterface guiObject)
    {
        if (GUIManager.Instance != null)
        {
            GUIManager.Instance.InternalUnregisterGUIObject(guiObject);
        }
    }

    private void InternalUnregisterGUIObject(GUIInterface guiObject)
    {
        List<GUIInterface> list;
        if (m_GUIObjects.TryGetValue(guiObject.GetOrder(), out list))
        {
            list.Remove(guiObject);
        }
    }

    public void OnGUI()
    {
        for (int i = 0; i < m_GUIObjects.Count; ++i)
        {
            List<GUIInterface> guiInterfaces = m_GUIObjects[m_GUIObjects.Keys[i]];
            for(int j = 0; j < guiInterfaces.Count; ++j)
            {
                guiInterfaces[j].OnOrderedGUI();
            }
        }
    }
}
