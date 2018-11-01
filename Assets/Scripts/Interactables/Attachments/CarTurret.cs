using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTurret : CarAttachment {
    
    public Collider carBodyCollider;
    public GameObject bulletPrefab;
    public float fireRate;
    public float bulletSpeed;
    public float scrapCostPerBullet = 1f;

    private float fireCooldown;

	// Update is called once per frame
	void Update () {
        fireCooldown = Mathf.Max(0, fireCooldown - Time.deltaTime);
        // Debug.Log("B: " + beingUsed + "F: " + fireCooldown);
		if(beingUsed && fireCooldown <= 0 && car.PayScrap(scrapCostPerBullet))
        {
            fireCooldown += 1 / fireRate;
            var bulletObject = GameObject.Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var bulletRbody = bulletObject.GetComponent<Rigidbody>();
            bulletRbody.velocity = car.rbody.velocity + bulletSpeed * transform.forward;

            // Cannot shoot self
            // At some point, the turret should also restrict the look angles of the player so they can't for instance look straight down and fire
            Physics.IgnoreCollision(bulletObject.GetComponent<Collider>(), carBodyCollider);
        }
	}
}
