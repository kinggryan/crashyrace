using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBooster : CarAttachment {

    public float boostForce;
    public float boostDuration;
    public float maxBoostedSpeed;
    public float scrapCost;

    private float boostTimer;

	// Update is called once per frame
    void Update() {
        if (boostTimer >= 0 && car.rbody.velocity.magnitude < maxBoostedSpeed)
        {
            car.rbody.AddForce(boostForce * transform.forward);
            boostTimer -= Time.deltaTime;
        }
	}

    public override void Use(GrapplerCharacterController character)
    {
        if(car.PayScrap(scrapCost))
        {
            boostTimer = boostDuration;
            base.Use(character);
        }
    }
}
