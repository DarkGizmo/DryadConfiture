using System;
using UnityEngine;

class JumpBehavior : Behavior
{
    public float TimeBetweenJump = 3.0f;
    public float JumpForce = 1.0f;

    private float TimeLeftBeforeJump;

    public override void Activate()
    {
        m_Active = true;
        TimeLeftBeforeJump = TimeBetweenJump;
    }

    public override void Deactivate()
    {
        m_Active = false;
    }

    public void Jump()
    {
        GetComponent<Rigidbody2D>().velocity += Vector2.up * JumpForce;
        GetComponent<GroundController>().SetIsGrounded(false);
    }

    public void Update()
    {
        if(m_Active)
        {
            TimeLeftBeforeJump -= TimeHelper.GameTime;
            if(TimeLeftBeforeJump <= 0.0f)
            {
                if (GetComponent<GroundController>().IsGrounded())
                {
                    Jump();
                }
                TimeLeftBeforeJump += TimeBetweenJump;
            }
        }
    }
}
