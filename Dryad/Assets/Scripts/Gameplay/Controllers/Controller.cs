using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public string ControllerName;
    protected bool m_ControllerActive;

    public abstract void OnActivated();
    public abstract void OnDeactivated();

    public void ActivateController()
    {
        m_ControllerActive = true;
        OnActivated();
        enabled = m_ControllerActive;
    }

    public void DeactivateController()
    {
        m_ControllerActive = false;
        OnDeactivated();
        enabled = m_ControllerActive;
    }
}
