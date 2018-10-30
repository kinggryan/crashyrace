using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerDecisionLayerActionFireAtEnemy : GrapplerDecisionLayer.Action
{
    Rigidbody target;
    CarAttachment turretBase;
    CarAttachment turret;
    const float maxPriorityDistance = 10f;
    const float minPriorityDistance = 100f;

    public GrapplerDecisionLayerActionFireAtEnemy(Car car, GrapplerCharacterController controller, Rigidbody target, CarAttachment turret, CarAttachment turretBase) : base(car, controller)
    {
        this.target = target;
        this.turret = turret;
        this.turretBase = turretBase;
    }

    public override float GetPriority()
    {
        var distance = Vector3.Distance(target.position, car.transform.position);
        var scalar = Mathf.Min(1,Mathf.Max(0,(distance - maxPriorityDistance)) / (minPriorityDistance - maxPriorityDistance));
        return 100 * (1-scalar);
    }

    public override void Update()
    {

    }

    // When called, the action should tell the steering layer to execute
    public override void Execute(GrapplerSteeringLayer steeringLayer)
    {
        // Check to see if the controller is currently using the turret
        // if so, just hand it the aim instruction
        // if not, hand it the "get in turret" action followed by the aim and fire instruction
        var cam = controller.GetComponentInChildren<Camera>();
        var instructions = new List<GrapplerSteeringLayer.State>();
        if (controller.selectedAttachment != turret)
        {
            var getInTurretInstruction = new GrapplerSteeringStateUseAttachment(controller, cam, turretBase);
            instructions.Add(getInTurretInstruction);
        }

        var fireInstruction = new GrapplerSteeringStateFireAtTarget(controller, cam, target);
        instructions.Add(fireInstruction);

        steeringLayer.SetInstructions(instructions);
    }
}
