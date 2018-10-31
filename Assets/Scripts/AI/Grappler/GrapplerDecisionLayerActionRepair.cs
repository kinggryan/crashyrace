using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerDecisionLayerActionRepair : GrapplerDecisionLayer.Action
{
    const float maxPriorityDistance = 100f;
    const float minPriorityDistance = 10f;

    public GrapplerDecisionLayerActionRepair(Car car, GrapplerCharacterController controller) : base(car,controller)
    {

    }

    public override float GetPriority()
    {
        // The priority here scales by how damaged you are and the distance from other cars
        // This way, if you're close to an enemy you prioritize repairing less, as other things are likely more important
        // If you're far away, you need to repair the damage to catch up, so this gains a high priority
        var target = car.ClosestEnemyCar();
        var distance = Vector3.Distance(target.transform.position, car.transform.position);
        var scalar = Mathf.Min(1, Mathf.Max(0, (distance - minPriorityDistance)) / (maxPriorityDistance - minPriorityDistance));
        var damageAmount = 1 - car.hp / car.maxHP;
       
        return 100 * 100 * scalar*damageAmount;
    }

    public override void Update()
    {

    }

    // When called, the action should tell the steering layer to execute
    public override void Execute(GrapplerSteeringLayer steeringLayer)
    {
        if(car.damageObjects.Count == 0)
        {
            Debug.LogError("Tried to execute repair action when the car has no damage objects.");
            return;
        }
        // Check to see if the controller is currently using the turret
        // if so, just hand it the aim instruction
        // if not, hand it the "get in turret" action followed by the aim and fire instruction
        var cam = controller.GetComponentInChildren<Camera>();
        var instructions = new List<GrapplerSteeringLayer.State>();
        var damageObject = car.damageObjects[0];
        var getInTurretInstruction = new GrapplerSteeringStateUseAttachment(controller, cam, damageObject);
        instructions.Add(getInTurretInstruction);

        steeringLayer.SetInstructions(instructions);
    }
}
