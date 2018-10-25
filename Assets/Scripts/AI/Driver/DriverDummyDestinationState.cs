using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DummyDestinationState : DriverDecisionLayer.State {

    public Vector3[] points;
    private int currentPointIndex = 0;

    public DummyDestinationState(DriverSteeringLayer steeringLayer, DriverDecisionLayer decisionLayer, Vector3[] points) : base(steeringLayer, decisionLayer)
    {
        this.points = points;
        steeringLayer.SetDestination(points[currentPointIndex]);
    }

    // Update is called once per frame
    public override DriverDecisionLayer.State Update () {
		if(Vector3.Distance(decisionLayer.transform.position, points[currentPointIndex]) < 10f)
        {
            currentPointIndex = (currentPointIndex+1) % points.Length;
            steeringLayer.SetDestination(points[currentPointIndex]);
        }
        return null;
	}

    public override DriverDecisionLayer.State DidDropPickup(Pickup pickup)
    {
        if(pickup is OrbPickup)
        {
            return new DriverDecisionStatePursuit(steeringLayer, decisionLayer, pickup.transform);
        }

        return null;
    }
}
