using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    public static List<Waypoint> waypoints { get; private set; }

    private Collider trigger;
    private List<Car> disabledForCars = new List<Car>();

    private void Awake()
    {
        trigger = GetComponent<Collider>();
        if (waypoints == null)
            waypoints = new List<Waypoint>();
        waypoints.Add(this);
    }

    // Use this for initialization
    private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponentInParent<Car>();
        car.EnteredWaypoint(this);
    }

    public void DisableForCar(Car car)
    {
        StartCoroutine(DisableForCarForSeconds(car,60));
    }

    public bool IsEnabledForCar(Car car)
    {
        return !disabledForCars.Contains(car);
    }

    IEnumerator DisableForCarForSeconds(Car car, float seconds)
    {
        disabledForCars.Add(car);
        Physics.IgnoreCollision(car.carBodyCollider, trigger, true);
        yield return new WaitForSeconds(seconds);

        disabledForCars.Remove(car);
        Physics.IgnoreCollision(car.carBodyCollider, trigger, false);
    }

    public static List<Waypoint> WaypointsForCar(Car car)
    {
        var otherWaypoints = new List<Waypoint>();
        foreach(var waypoint in waypoints)
        {
            if(waypoint.IsEnabledForCar(car))
            {
                otherWaypoints.Add(waypoint);
            }
        }

        return otherWaypoints;
    }
}
