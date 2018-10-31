using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPickup : Pickup {

    public override void TakeDamage(float damage, float carHP)
    {
        if (carHP <= 10f)
        {
            car.DropPickup(this);
        }
    }

    public override void AttachToCar(Car car, Vector3 relativePosition)
    {
        base.AttachToCar(car, relativePosition);

        car.RepairDamage(100);
    }

    protected override void Update()
    {
        base.Update();

        if (transform.position.y < 0)
        {
            // TODO: A smarter version of this
            transform.position += 50 * Vector3.up;
        }
    }
}
