using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class TimedLogicController : LogicController
{
    public float IntervalDuration = 10.0f;
    private float m_IntervalTimeLeft;

    public void Start()
    {
        m_IntervalTimeLeft = IntervalDuration;
    }

    public void Update()
    {
        m_IntervalTimeLeft -= TimeHelper.GameTime;

        while(m_IntervalTimeLeft < 0.0f)
        {
            m_IntervalTimeLeft += IntervalDuration;
            TriggerActivation();
        }
    }
}
