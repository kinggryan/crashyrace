using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverCharacterController : ICarControlInput {

    public Camera cam;
    public MouseLook mouseLook;
    public float maxControllerLookAngle = 90;
    public float controllerLookSpeed = 15f;
    public int playerNum;
    public bool controllerMode;
    

    public void Update()
    {
        if (controllerMode)
            UpdateLookDirectionForController();
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

    void UpdateLookDirectionForController()
    {
        var desiredHorizontalLookDirection = maxControllerLookAngle*Input.GetAxis("LookHorizontal_" + playerNum);
        var desiredVerticalLookDirection = maxControllerLookAngle * Input.GetAxis("LookVertical_" + playerNum);
        var desiredRotation = Quaternion.AngleAxis(desiredHorizontalLookDirection, Vector3.up) * Quaternion.AngleAxis(-desiredVerticalLookDirection, Vector3.right);
        cam.transform.localRotation = Quaternion.RotateTowards(cam.transform.localRotation, desiredRotation, controllerLookSpeed * Time.deltaTime);
    }
}
