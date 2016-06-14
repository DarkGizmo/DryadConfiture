using UnityEngine;

abstract class Behavior : MonoBehaviour
{
    public bool m_Active;

    public void Start()
    {
        if(m_Active)
        {
            Activate();
        }
    }

    abstract public void Activate();
    abstract public void Deactivate();
}
