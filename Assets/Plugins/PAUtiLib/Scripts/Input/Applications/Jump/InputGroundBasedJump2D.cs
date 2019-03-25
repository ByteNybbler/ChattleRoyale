// Author(s): Paul Calande
// Input component that allows for jumping.
// For the event invoked when jumping, subscribe to GroundBasedJump2D.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGroundBasedJump2D// : InputDistributed
{
    //[SerializeField]
    [Tooltip("The component to use for jumping.")]
    GroundBasedJump2D groundBasedJump;
    //[SerializeField]
    [Tooltip("The vertical velocity with which the Rigidbody jumps.")]
    float jumpVelocity;
    //[SerializeField]
    [Tooltip("What button to press to jump.")]
    KeyCode buttonJump = KeyCode.W;

    public InputGroundBasedJump2D(GroundBasedJump2D groundBasedJump,
        float jumpVelocity, KeyCode buttonJump)
    {
        this.groundBasedJump = groundBasedJump;
        this.jumpVelocity = jumpVelocity;
        this.buttonJump = buttonJump;
    }

    //public override void ReceiveInput(InputReader inputReader)
    public void Tick()
    {
        if (Input.GetKeyDown(buttonJump))
        {
            // Try to jump.
            groundBasedJump.TryJump(jumpVelocity);
        }
    }
}