using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerSteeringStateFireAtTarget : GrapplerSteeringLayer.State
{
    Rigidbody target;
    const float bulletVelocity = 50f;
    
    public GrapplerSteeringStateFireAtTarget(GrapplerCharacterController controller, Camera cam, Rigidbody target) : base(controller, cam)
    {
        this.target = target;
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
        return true;
    }
    public override bool GetUseButtonUp()
    {
        return false;
    }
    public override bool Update()
    {
        // calculate vertical arc and lead time
        var gravity = Physics.gravity;
        var distance = Vector3.Distance(controller.transform.position, target.position);
        var upwardAngle = Mathf.Rad2Deg*Mathf.Asin(distance * gravity.magnitude / (bulletVelocity* bulletVelocity)) / 2;
        
        // Calculate lead tracking
        var enemyVelocitySelfSpace = target.velocity - controller.car.velocity;
        var horizontalLookTarget = target.transform.position;
        horizontalLookTarget.y = controller.transform.position.y;
        var interceptionPoint = Utilities.FirstOrderIntercept(controller.transform.position, Vector3.zero, bulletVelocity, horizontalLookTarget, enemyVelocitySelfSpace);

        cam.transform.LookAt(interceptionPoint);
        cam.transform.Rotate(cam.transform.right, upwardAngle);
        return false;
    }


}
