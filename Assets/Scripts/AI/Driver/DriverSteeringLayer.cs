using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DriverSteeringLayer : ICarControlInput
{

    public Vector3 destination;
    public Rigidbody car;
    public float maxTurnAngle;

    private NavMeshPath path;
    private float steeringValue = 0;

	// Use this for initialization
	void Awake () {
        path = new NavMeshPath();
        NavMeshHit destinationNavMeshPointInfo;
        var foundPoint = NavMesh.SamplePosition(destination, out destinationNavMeshPointInfo, 4f, NavMesh.AllAreas);
        if (!foundPoint)
            Debug.LogError("Couldn't find point at start");
        destination = destinationNavMeshPointInfo.position;
    }
	
	// Update is called once per frame
	void Update () {
        // Recalculate our navmesh path
        NavMeshHit startNavMeshPointInfo;
        var foundPoint = NavMesh.SamplePosition(car.transform.position, out startNavMeshPointInfo, 4f, NavMesh.AllAreas);
        if (!foundPoint)
            Debug.LogError("Couldn't find point...");

        var foundPath = NavMesh.CalculatePath(startNavMeshPointInfo.position, destination, NavMesh.AllAreas, path);
        if (!foundPath)
            Debug.LogError("Error finding path...");

        // Calculate angle between our current velocity and the vector towards the destination, projected onto the vector of the car's steering plane
        if(path.corners.Length > 1)
        {
            var desiredMovementVector = path.corners[1] - car.position;
            var currentMovementVector = car.velocity;
            
            var correctedDesiredMovementVector = Vector3.ProjectOnPlane(desiredMovementVector, car.transform.up);
            var correctedCurrentMovementVector = Vector3.ProjectOnPlane(currentMovementVector, car.transform.up);
            var steeringAngle = Vector3.SignedAngle(correctedCurrentMovementVector, correctedDesiredMovementVector, car.transform.up);
            steeringValue = Mathf.Clamp(steeringAngle / maxTurnAngle, -1, 1);
        } else
        {
            Debug.LogError("Path not long enough...");
        }
	}

    public override float GetMotorInput()
    {
        return 1f;
    }
    public override float GetSteerInput()
    {
        return steeringValue;
    }
    public override float GetBrakeInput()
    {
        return 0;
    }
}
