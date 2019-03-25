// Author(s): Paul Calande
// Input component for walking on the ground.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGroundBasedWalk2D// : InputDistributed
{
    //[SerializeField]
    [Tooltip("Reference to the ground-based acceleration to use for walking.")]
    GroundBasedAcceleration2D acceleratorWalk;
    //[SerializeField]
    [Tooltip("How much to accelerate by.")]
    float acceleration;
    //[SerializeField]
    [Tooltip("Input name.")]
    string inputName = "Horizontal";

    public InputGroundBasedWalk2D(GroundBasedAcceleration2D acceleratorWalk,
        float acceleration, string inputName = "Horizontal")
    {
        this.acceleratorWalk = acceleratorWalk;
        this.acceleration = acceleration;
        this.inputName = inputName;
    }

    //public override void ReceiveInput(InputReader inputReader)
    public void Tick()
    {
        //float axisH = inputReader.GetAxisHorizontalRaw();
        float axisH = Input.GetAxisRaw(inputName);
        acceleratorWalk.ApplyHorizontalAcceleration(axisH * acceleration);
    }
}