// Author(s): Paul Calande
// Input component that supports variable jumping.
// This means that the longer the player holds the jump button, the higher they go.
// Letting go of the jump button before the peak of the jump will decrease the
// overall height of the jump.
// This component only enables the variable jumping component of jumping.
// An actual input-based jump component is still necessary to actually jump.
// Additionally, other components such as GroundBasedVariableJump are needed
// to notify this component that variable jumping is allowed for a given jump.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputVariableJump2D// : InputDistributed
{
    //[SerializeField]
    [Tooltip("Tracks the direction along which velocity will be removed.")]
    VelocityInUpSpace2D vius;
    //[SerializeField]
    [Tooltip("Reference to the player's gravity component.")]
    Gravity2D gravity;
    //[SerializeField]
    [Tooltip("What the vertical velocity is multiplied by when the variable jump occurs.")]
    float variableJumpDampFactor = 0.5f;
    //[SerializeField]
    [Tooltip("What button to press to jump.")]
    KeyCode buttonJump = KeyCode.W;

    // Whether variable jumping has occurred yet.
    // It can only occur if this variable is false.
    // When variable jumping occurs, this variable becomes true.
    // This makes sure the player can't trigger variable jumping multiple times per jump.
    bool variableJumped = true;

    public InputVariableJump2D(VelocityInUpSpace2D vius, Gravity2D gravity,
        float variableJumpDampFactor, KeyCode buttonJump)
    {
        this.vius = vius;
        this.gravity = gravity;
        this.variableJumpDampFactor = variableJumpDampFactor;
        this.buttonJump = buttonJump;
    }

    //public override void ReceiveInput(InputReader inputReader)
    public void Tick()
    {
        if (Input.GetKeyUp(buttonJump))
        {
            // Do not allow variable jumping in zero gravity.
            if (!variableJumped && gravity.GetAcceleration() != 0.0f)
            {
                Vector2 velocity = vius.GetVelocity();
                if (velocity.y > 0.0f)
                {
                    velocity.y *= variableJumpDampFactor;
                    vius.SetVelocity(velocity);
                    // Variable jump executed successfully.
                    variableJumped = true;
                }
            }
        }
    }

    // Calling this method makes it possible to trigger variable jumping again.
    // This should be called when something like a double jump is triggered.
    public void ResetVariableJump()
    {
        variableJumped = false;
    }
}