using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverDummyDestinationLayer : MonoBehaviour {

    public DriverSteeringLayer steeringLayer;
    public Vector3[] points;
    private int currentPointIndex = 0;

	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(transform.position, points[currentPointIndex]) < 10f)
        {
            currentPointIndex = (currentPointIndex+1) % points.Length;
            steeringLayer.SetDestination(points[currentPointIndex]);
        }
	}
}
