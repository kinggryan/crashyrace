using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverCharacterController : ICarControlInput {

    public Camera cam;
    public MouseLook mouseLook;
    public ControllerLook controllerLook;
    public int playerNum;
    public bool controllerMode;
    
    public void Update()
    {
        if (controllerMode)
            controllerLook.LookRotation(cam, playerNum);
        else
            mouseLook.LookRotation(transform, cam.transform);
    }

    public override float GetMotorInput()
    {
        if (controllerMode)
            return Input.GetAxis("ActionA_" + playerNum);
        else
            return Input.GetAxis("Vertical");
    }
    public override float GetSteerInput()
    {
        if (controllerMode)
            return Input.GetAxis("Horizontal_" + playerNum);
        else
            return Input.GetAxis("Horizontal");
    }
    public override float GetBrakeInput()
    {
        if (controllerMode)
            return Input.GetAxis("ActionB_" + playerNum);
        else
            return (Input.GetButton("Brake") ? 1 : 0);
    }
}
