using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDamage : CarAttachment {

    public float damage;
    public float repairAmount = 5;
    public float repairRate = 1;
    public Car car;

	public override void Use(GrapplerCharacterController character)
    {
        base.Use(character);

        damage -= repairAmount * repairRate * Time.deltaTime;
        if(damage <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
