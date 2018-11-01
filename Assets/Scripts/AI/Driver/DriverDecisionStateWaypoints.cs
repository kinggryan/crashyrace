using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DriverDecisionStateWaypoints : DriverDecisionLayer.State
{
    Waypoint targetWaypoint;
    Car car;

    const float maximumStartingAngle = 80f;

    public DriverDecisionStateWaypoints(DriverSteeringLayer steeringLayer, DriverDecisionLayer decisionLayer) : base(steeringLayer, decisionLayer)
    {
        car = steeringLayer.GetComponentInParent<Car>();
    }

    // Update is called once per frame
    public override DriverDecisionLayer.State Update()
    {
        // If we no longer have the orb, go for the orb!
        // TODO: Probably better to do this as a response to dropping a pickup, not on update, since its more processing than is needed
        if(!car.HasOrb())
        {
            return new DriverDecisionStatePursuit(steeringLayer, decisionLayer, Object.FindObjectOfType<OrbPickup>().transform);
        }
        
        if (targetWaypoint == null || !targetWaypoint.IsEnabledForCar(car))
            CalculateTargetWaypoint();

        steeringLayer.SetDestination(targetWaypoint.transform.position);
        return null;
    }

    void CalculateTargetWaypoint()
    {
        Waypoint targetWaypoint = null;
        Waypoint fallbackWaypoint = null;
        var shortestPathLength = Mathf.Infinity;
        var shortestFallbackPathLength = Mathf.Infinity;

        // For each waypoint, calculate a path there
        NavMeshHit hitInfo;
        NavMesh.SamplePosition(car.transform.position, out hitInfo, Mathf.Infinity, NavMesh.AllAreas);

        var startPosition = hitInfo.position;
        var carrbody = steeringLayer.car;
        var velocity = carrbody.velocity;
        foreach(var waypoint in Waypoint.WaypointsForCar(car))
        {
            // Find the end point
            NavMesh.SamplePosition(waypoint.transform.position, out hitInfo, Mathf.Infinity, NavMesh.AllAreas);
            NavMeshPath path = new NavMeshPath();
            var foundPath = NavMesh.CalculatePath(startPosition, hitInfo.position, NavMesh.AllAreas, path);
            if (!foundPath)
                Debug.LogError("Error finding path to " + waypoint);
            else
            {
                //  Determine if the path starts by going in the direction we are already going
                var initialDirection = path.corners[Mathf.Min(1, path.corners.Length - 1)] - carrbody.position;
                
                //  if so, then see if it's the shortest qualifying path
                // move along the shortest path moving in the correct direction
                var pathLength = 0f;
                for (var i = 0; i < path.corners.Length - 2; i++)
                {
                    pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                }

                // Do the fallback waypoint
                if (Vector3.Angle(velocity, initialDirection) <= maximumStartingAngle)
                {
                    if (pathLength < shortestPathLength)
                    {
                        shortestPathLength = pathLength;
                        targetWaypoint = waypoint;
                    }
                } else
                {
                    if (pathLength < shortestFallbackPathLength)
                    {
                        shortestFallbackPathLength = pathLength;
                        fallbackWaypoint = waypoint;
                    }
                }
                
            }
        }

        this.targetWaypoint = targetWaypoint != null ? targetWaypoint : fallbackWaypoint;
    }
}
