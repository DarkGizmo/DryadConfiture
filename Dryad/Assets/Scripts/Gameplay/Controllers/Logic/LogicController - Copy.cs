using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class LogicController : MonoBehaviour
{
    public List<string> ActivationList;
    public List<string> DeactivationList;
    public bool InverseListOnActivation = false;

    private bool m_Inversed = false;

    public void TriggerActivation()
    {
        for(int i = 0; i < ActivationList.Count; ++i)
        {
            SetControllerActive(ActivationList[i], !m_Inversed);
        }

        for (int i = 0; i < DeactivationList.Count; ++i)
        {
            SetControllerActive(DeactivationList[i], m_Inversed);
        }

        m_Inversed = !m_Inversed;
    }

    private void SetControllerActive(string name, bool active)
    {
        Controller[] controllers = GetComponents<Controller>();
        for(int i = 0; i < controllers.Length; ++i)
        {
            if (String.Compare(controllers[i].ControllerName, name) == 0)
            {
                if (active)
                {
                    controllers[i].ActivateController();
                }
                else
                {
                    controllers[i].DeactivateController();
                }
            }
        }
    }
}
