using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBooster : CarAttachment {

    public float boostForce;
    public Rigidbody car;
    public float maxBoostedSpeed;

	// Update is called once per frame
    void Update() {
        if (beingUsed && car.velocity.magnitude < maxBoostedSpeed)
        {
            car.AddForce(boostForce * transform.forward);
        }
	}
}
