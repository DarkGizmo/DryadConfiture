using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SimpleGroundController : GroundController
{
    public float Speed = 0.0f;
    public override float GetVelocitySide()
    {
        if(IsGrounded())
        {
            return Speed;
        }

        return 0.0f;
    }
}