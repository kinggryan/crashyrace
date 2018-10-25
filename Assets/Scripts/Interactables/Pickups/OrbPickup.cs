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
    
}
