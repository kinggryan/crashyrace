using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerDecisionLayer : MonoBehaviour, IGrapplerSteeringLayerDelegate {

    public abstract class Action
    {
        protected Car car;
        protected GrapplerCharacterController controller;
        
        public Action(Car car, GrapplerCharacterController controller)
        {
            this.car = car;
            this.controller = controller;
        }

        // Priorities should generally be a number from 0 to 100.
        public abstract float GetPriority();
        public abstract void Update();
        // When called, the action should tell the steering layer to execute
        public abstract void Execute(GrapplerSteeringLayer steeringLayer);
    }

    public GrapplerSteeringLayer steeringLayer;
    public Car car;
    public GrapplerCharacterController controller;
    public Rigidbody enemyCar;
    public CarAttachment turret;
    public CarAttachment turretBase;

    private List<Action> actions = new List<Action>();
    private Action currentAction = null;
    private float currentActionPriority = 0;

    const float priorityTransitionThreshold = 10;

    // Use this for initialization
    void Awake () {
        var fireAtEnemyAction = new GrapplerDecisionLayerActionFireAtEnemy(car, controller, enemyCar, turret, turretBase);
        actions.Add(fireAtEnemyAction);

        steeringLayer.del = this;
	}
	
	// Update is called once per frame
	void Update () {
        UpdateCurrentAction();
	}

    void UpdateCurrentAction()
    {
        Action tempMaxAction = null;
        var tempMaxActionPriority = 0f;

        foreach (var action in actions)
        {
            // Update the action and then get it's priority
            action.Update();

            // Find the max priority action
            var priority = action.GetPriority();
            if(priority > tempMaxActionPriority)
            {
                tempMaxAction = action;
                tempMaxActionPriority = priority;
            }

            // Update the current action's priority here also so we don't have to recalculate it later
            if(action == currentAction)
            {
                currentActionPriority = priority;
            }
        }

        // If this is a high enough priority to exceed the threshold, then change our action
        if(tempMaxActionPriority > currentActionPriority + priorityTransitionThreshold)
        {
            currentAction = tempMaxAction;
            currentActionPriority = tempMaxActionPriority;
            currentAction.Execute(steeringLayer);
        }
    }

    // Steering layer delegate

    public void DidCompleteAllSteeringInstructions()
    {
        currentAction.Execute(steeringLayer);
    }
}
