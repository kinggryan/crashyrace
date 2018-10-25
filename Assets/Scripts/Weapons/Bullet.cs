using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	private void OnCollisionEnter(Collision collision)
    {
        var car = collision.collider.GetComponentInParent<Car>();
        if(car != null)
        {
            var rbody = GetComponent<Rigidbody>();
            car.TakeDamage(collision.contacts[0].point, rbody.velocity, 1f);
        }
        GameObject.Destroy(gameObject);
    }
}
