using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ControllerLook {

    public bool clampHorizontalLook = true;
    public float maxHorizontalLookAngle;
    public float maxVerticalLookAngle;
    public float lookSpeed;
    public float unclampedLookSpeed;

    // Use this for initialization
    public void LookRotation(Camera cam, int playerNum)
    {
        if(clampHorizontalLook)
        {
            var desiredHorizontalLookDirection = maxHorizontalLookAngle * Input.GetAxis("LookHorizontal_" + playerNum);
            var desiredVerticalLookDirection = maxVerticalLookAngle * Input.GetAxis("LookVertical_" + playerNum);
            var desiredRotation = Quaternion.AngleAxis(desiredHorizontalLookDirection, Vector3.up) * Quaternion.AngleAxis(-desiredVerticalLookDirection, Vector3.right);
            cam.transform.localRotation = Quaternion.RotateTowards(cam.transform.localRotation, desiredRotation, lookSpeed * Time.deltaTime);
        } else
        {
            var desiredHorizontalLookDirection = maxHorizontalLookAngle * Input.GetAxis("LookHorizontal_" + playerNum);
            var desiredVerticalLookDirection = maxVerticalLookAngle * Input.GetAxis("LookVertical_" + playerNum);
            var desiredRotation = Quaternion.AngleAxis(cam.transform.localEulerAngles.y + desiredHorizontalLookDirection, Vector3.up) * Quaternion.AngleAxis(cam.transform.localEulerAngles.x - desiredVerticalLookDirection, Vector3.right);
            cam.transform.localRotation = Quaternion.RotateTowards(cam.transform.localRotation, desiredRotation, unclampedLookSpeed * Time.deltaTime);
        }
    }
}
