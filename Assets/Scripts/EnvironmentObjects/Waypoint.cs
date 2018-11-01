using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    Collider trigger;

    private void Awake()
    {
        trigger = GetComponent<Collider>();
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

    IEnumerator DisableForCarForSeconds(Car car, float seconds)
    {
        Physics.IgnoreCollision(car.carBodyCollider, trigger, true);
        yield return new WaitForSeconds(seconds);

        Physics.IgnoreCollision(car.carBodyCollider, trigger, false);
    }
}
