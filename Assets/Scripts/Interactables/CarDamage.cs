using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDamage : CarAttachment {
    
    public float maxDamage = 15;
    public float repairAmount = 5;

    private float damage;
    private Car car;

    public void Init(float damage, Car car)
    {
        this.damage = damage;
        this.car = car;
    }

    // Takes that much damage and returns the remaining damage
    public float TakeDamage(float damage)
    {
        this.damage += damage;
        if(this.damage >= maxDamage)
        {
            var delta = this.damage - maxDamage;
            this.damage = maxDamage;
            return delta;
        }
        return 0;
    }

    public override void Use(GrapplerCharacterController character)
    {
        base.Use(character);

        damage -= repairAmount;
        car.RepairDamage(repairAmount);
        if(damage <= 0)
        {
            car.RemoveDamageObject(this);
        }
    }
}
