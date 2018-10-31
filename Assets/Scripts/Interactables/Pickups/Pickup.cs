using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    public Collider trigger;
    public Collider collider;
    public Rigidbody rbody;
    public Car car;

    private Vector3 relativePositionToCar;

	// Use this for initialization
	private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponentInParent<Car>();
        if(car != null)
        {
            car.AcquirePickup(this);
        }
    }

    public virtual void TakeDamage(float damage, float carHP)
    {

    }

    protected virtual void Update()
    {
        if (car != null)
            transform.position = car.transform.TransformPoint(relativePositionToCar);
    }

    public virtual void AttachToCar(Car car, Vector3 relativePosition)
    {
        this.car = car;
        relativePositionToCar = relativePosition;
    }

    public virtual void RemoveFromCar()
    {
        this.car = null;
    }

    public void TemporarilyDisableCollisionsWith(Collider col)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), col);
        EnableCollisions(col, 2f);
    }

    public void SetPickupEnabled(bool enabled)
    {
        trigger.enabled = enabled;
        collider.enabled = enabled;
        rbody.isKinematic = !enabled;
    }

    private IEnumerator EnableCollisions(Collider col, float time)
    {
        yield return new WaitForSeconds(time);

        Physics.IgnoreCollision(GetComponent<Collider>(), col, false);
    }
}
