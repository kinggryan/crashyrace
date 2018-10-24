using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTurret : CarAttachment {

    public Rigidbody car;
    public GameObject bulletPrefab;
    public float fireRate;
    public float bulletSpeed;

    private float fireCooldown;

	// Update is called once per frame
	void Update () {
        fireCooldown = Mathf.Max(0, fireCooldown - Time.deltaTime);
        // Debug.Log("B: " + beingUsed + "F: " + fireCooldown);
		if(beingUsed && fireCooldown <= 0)
        {
            fireCooldown += 1 / fireRate;
            var bulletObject = GameObject.Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var bulletRbody = bulletObject.GetComponent<Rigidbody>();
            bulletRbody.velocity = car.velocity + bulletSpeed * transform.forward;
        }
	}
}
