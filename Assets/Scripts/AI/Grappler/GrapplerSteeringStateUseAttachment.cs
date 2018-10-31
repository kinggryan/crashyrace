using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerSteeringStateUseAttachment : GrapplerSteeringLayer.State {

    private CarAttachment targetAttachment;
    private float targetMovementPosition;
    private bool usedAttachment;

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
        return usedAttachment;
    }

    public override bool GetUseButtonUp()
    {
        return false;
    }

    public override bool Update()
    {
        // If for some reason the target attachment has been destroyed, we should move to the next instruction
        if (targetAttachment == null)
            return true;

        cam.transform.LookAt(targetAttachment.transform.position);
        if(usedAttachment)
        {
            return true;
        }
        return false;
    }

    public override CarAttachment GetAttachmentToUse()
    {
        var currentPos = controller.movementPointPosition;
        //Debug.Log("Distance from " + targetAttachment + ": " + Mathf.Abs(currentPos - targetMovementPosition));
        if (Mathf.Abs(currentPos - targetMovementPosition) < 0.2f)
        {
            Debug.Log("Using: " + targetAttachment);
            usedAttachment = true;
            return targetAttachment;
        }

        return null;
    }
}
