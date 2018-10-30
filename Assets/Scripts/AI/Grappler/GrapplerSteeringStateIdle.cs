using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerSteeringStateIdle : GrapplerSteeringLayer.State
{
    public GrapplerSteeringStateIdle(GrapplerCharacterController controller, Camera cam) : base(controller, cam)
    {
        // No special initialization
    }
    public override float GetHorizontalInput()
    {
        return 0;
    }
    public override float GetVerticalInput()
    {
        return 0;
    }
    public override bool GetUseButtonDown()
    {
        return false;
    }
    public override bool GetUseButtonUp()
    {
        return false;
    }
    public override bool Update()
    {
        return false;
    }
}
