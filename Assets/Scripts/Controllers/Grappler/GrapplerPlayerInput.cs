using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerPlayerInput : GrapplerInput {

    public Camera cam;
    public MouseLook mouseLook;
    public ControllerLook controllerLook;
    public float maxUseDistance = 2f;
    public bool controllerMode;

    // Use this for initialization
    void Awake () {
        mouseLook = new MouseLook();
        mouseLook.Init(cam.transform);
    }
	
	// Update is called once per frame
	void Update () {
        if (controllerMode)
            controllerLook.LookRotation(cam, playerNum);
        else
            mouseLook.LookRotation(transform, cam.transform);
    }


    public override float GetHorizontalInput()
    {
        if (controllerMode)
            return Input.GetAxis("Horizontal_" + playerNum);
        else
            return Input.GetAxis("Horizontal");
    }

    public override float GetVerticalInput()
    {
        if (controllerMode)
            return Input.GetAxis("Vertical_" + playerNum);
        else
            return Input.GetAxis("Vertical");
    }

    public override bool GetUseButtonDown()
    {
        if (controllerMode)
            return Input.GetButtonDown("ActionA_" + playerNum);
        else
            return Input.GetButtonDown("Fire1");
    }

    public override bool GetUseButtonUp()
    {
        if (controllerMode)
            return Input.GetButtonUp("ActionA_" + playerNum);
        else
            return Input.GetButtonUp("Fire1");
    }
    public override bool ShouldLockRotationToCar()
    {
        return controllerMode;
    }
    public override CarAttachment GetAttachmentToUse()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, maxUseDistance, LayerMask.NameToLayer("attachments")))
        {
            var attachment = hitInfo.collider.GetComponent<CarAttachment>();
            return attachment;
        }

        return null;
    }
}
