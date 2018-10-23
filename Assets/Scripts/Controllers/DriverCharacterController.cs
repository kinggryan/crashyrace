using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverCharacterController : ICarControlInput {

	public override float GetMotorInput()
    {
        return Input.GetAxis("Vertical");
    }
    public override float GetSteerInput()
    {
        return Input.GetAxis("Horizontal");
    }
    public override float GetBrakeInput()
    {
        return (Input.GetButton("Brake") ? 1 : 0);
    }
}
