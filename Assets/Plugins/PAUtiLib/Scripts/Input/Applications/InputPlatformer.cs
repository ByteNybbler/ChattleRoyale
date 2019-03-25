// Author(s): Paul Calande
// Composite class for a player character in a platforming game.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPlatformer
{
    Mover2D mover;
    Rigidbody2D rb;

    UpDirection2D upDirection;
    VelocityInUpSpace2D vius;
    Gravity2D gravity;

    GroundChecker2D groundChecker;
    GroundBasedJump2D gbj;

    GroundBasedAcceleration2D gba;

    InputGroundBasedWalk2D igba;
    InputGroundBasedJump2D igbj;
    InputVariableJump2D variableJumping;

    public InputPlatformer(Mover2D mover, Rigidbody2D rb,
        Angle upAngle, float gravityAcceleration,
        Angle maxSlopeAngle, float groundDeceleration,
        float maxHorizontalSpeed, float groundAcceleration,
        string groundInputName, float jumpVelocity,
        KeyCode buttonJump, float variableJumpDampFactor)
    {
        upDirection = new UpDirection2D(upAngle);
        vius = new VelocityInUpSpace2D(mover, upDirection);
        gravity = new Gravity2D(vius, gravityAcceleration);

        groundChecker = new GroundChecker2D(rb, upDirection, maxSlopeAngle);

        gba = new GroundBasedAcceleration2D(groundChecker, vius, groundDeceleration, maxHorizontalSpeed);

        igba = new InputGroundBasedWalk2D(gba, groundAcceleration, groundInputName);

        gbj = new GroundBasedJump2D(groundChecker, vius);
        igbj = new InputGroundBasedJump2D(gbj, jumpVelocity, buttonJump);
        variableJumping = new InputVariableJump2D(vius, gravity, variableJumpDampFactor, buttonJump);
        gbj.SubscribeToJumped(x => variableJumping.ResetVariableJump());
    }

    public void Tick(float deltaTime)
    {
        gravity.Tick(deltaTime);

        groundChecker.Tick();
        gbj.Tick();

        gba.Tick(deltaTime);

        igba.Tick();
        igbj.Tick();
        variableJumping.Tick();
    }

    public void SubscribeToJumped(GroundBasedJump2D.JumpedHandler callback)
    {
        gbj.SubscribeToJumped(callback);
    }

    public void SubscribeToLanded(GroundChecker2D.GroundLandedHandler callback)
    {
        groundChecker.GroundLanded += callback;
    }
}