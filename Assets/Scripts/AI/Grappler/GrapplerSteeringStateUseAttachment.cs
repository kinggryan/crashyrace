using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerSteeringStateUseAttachment : GrapplerSteeringLayer.State {

    private CarAttachment targetAttachment;
    private float targetMovementPosition;

    public GrapplerSteeringStateUseAttachment(GrapplerCharacterController controller, Camera cam, CarAttachment attachment) : base(controller, cam)
    {
        this.targetAttachment = attachment;
        targetMovementPosition = controller.GetClosestMovementPositionForWorldSpacePoint(attachment.transform.position);
    }

    public override float GetHorizontalInput()
    {

        // Determine the current carEdgeSpacePos
        var currentPos = controller.movementPointPosition;

        // Determine the maxEdgeSpacePos
        var maxLength = controller.GetMovementPointMaxDistance();

        Debug.Log("Current pos :" + currentPos + " other pos :" + targetMovementPosition);

        // Determine if its a shorter path to move right or left
        if(targetMovementPosition > currentPos)
        {
            var rightDistance = targetMovementPosition - currentPos;
            var leftDistance = maxLength - targetMovementPosition + currentPos;
            return rightDistance > leftDistance ? 1 : -1;
        } else
        {
            var leftDistance = currentPos - targetMovementPosition;
            var rightDistance = maxLength - currentPos + targetMovementPosition;
            return rightDistance > leftDistance ? 1 : -1;
        }
    }

    public override float GetVerticalInput()
    {
        return 0f;
    }

    public override bool GetUseButtonDown()
    {
        return false;
    }

    public override bool GetUseButtonUp()
    {
        return false;
    }

    public override GrapplerSteeringLayer.State Update()
    {
        return null;
    }
}
