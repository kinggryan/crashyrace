﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverDecisionStatePursuit : DriverDecisionLayer.State {

    public Transform target; 

    public DriverDecisionStatePursuit(DriverSteeringLayer steeringLayer, DriverDecisionLayer decisionLayer, Transform target) : base(steeringLayer,decisionLayer)
    {
        this.target = target;
    }

    // Update is called once per frame
    public override DriverDecisionLayer.State Update () {
        steeringLayer.SetDestination(target.position);
        return null;
	}

    public override DriverDecisionLayer.State DidAcquirePickup(Pickup pickup)
    {
        // Transition to escape layer
        var newState = new DummyDestinationState(steeringLayer, decisionLayer, new Vector3[] { new Vector3(100, 0, 410), new Vector3(-100,0,-100) });
        return newState;
    }
}