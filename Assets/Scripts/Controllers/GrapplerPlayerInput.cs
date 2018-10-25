using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerPlayerInput : GrapplerInput {

    public Camera cam;
    public MouseLook mouseLook;

    // Use this for initialization
    void Awake () {
        mouseLook = new MouseLook();
        mouseLook.Init(cam.transform);
    }
	
	// Update is called once per frame
	void Update () {
        mouseLook.LookRotation(transform, cam.transform);
    }


    public override float GetHorizontalInput()
    {
        return Input.GetAxis("Horizontal");
    }

    public override float GetVerticalInput()
    {
        return Input.GetAxis("Vertical");
    }

    public override bool GetUseButtonDown()
    {
        return Input.GetButtonDown("Fire1");
    }

    public override bool GetUseButtonUp()
    {
        return Input.GetButtonUp("Fire1");
    }
}
