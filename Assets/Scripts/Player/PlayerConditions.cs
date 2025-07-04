using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConditions
{
    // Conditions
    public bool IsGrounded { get; set; }
    public bool IsSprinting { get; set; }
    public bool IsWalking { get; set; }

    public void Reset()
    {
        IsSprinting = false;
    }

}
