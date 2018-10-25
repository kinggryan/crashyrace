using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

    [System.Serializable]
    public struct HPBracket
    {
        public float hp;
        public float maxSpeed;
    }

    public Rigidbody rbody;
    public Collider carBodyCollider;
    public SimpleCarController carController;
    public float maxHP = 100f;
    public HPBracket[] hpBrackets;

    public float outOfHPExplosionForce;

    public GameObject damagePrefab;

    float hp;
    private List<CarDamage> damageObjects = new List<CarDamage>();
    private List<Pickup> pickups = new List<Pickup>();

	// Use this for initialization
	void Awake () {
        hp = maxHP;
	}
	
	public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        // For this debug thing
        var previousBracket = BracketForHP(hp);
        hp -= damage;
        var newBracket = BracketForHP(hp);
        if(!Mathf.Approximately(newBracket.hp, previousBracket.hp))
        {
            ExplodeAtPoint(point);
            UpdateStatsForHPBracket(newBracket);
        }

        if(damageObjects.Count == 0)
        {
            CreateDamageObject(point, damage);
        } else
        {
            var remainingDamage = damageObjects[damageObjects.Count - 1].TakeDamage(damage);
            if(remainingDamage > 0)
            {
                CreateDamageObject(point, remainingDamage);
            }
        }

        foreach(var p in pickups)
        {
            p.TakeDamage(damage, hp);
        }
    }

    public void RepairDamage(float damage)
    {
        hp += damage;
    }

    public void RemoveDamageObject(CarDamage obj)
    {
        for(var i = 0; i < damageObjects.Count; i++)
        {
            if(damageObjects[i] == obj)
            {
                damageObjects.RemoveAt(i);
                break;
            }
        }
        GameObject.Destroy(obj.gameObject);
    }

    public void AcquirePickup(Pickup pickup)
    {
        // Disable the pickup's trigger
        // Place it in the center of the vehicle
        pickup.SetPickupEnabled(false);
        pickup.AttachToCar(this, 5 * Vector3.up);
        pickups.Add(pickup);
    }

    public void DropPickup(Pickup pickup)
    {
        Debug.Log("Dropped");
        pickup.SetPickupEnabled(true);
        pickup.RemoveFromCar();
        pickups.Remove(pickup);
    }

    private void CreateDamageObject(Vector3 point, float damage)
    {
        // Move the point slightly further out so its not like inside the car
        var extrudedPoint = carBodyCollider.ClosestPoint(point);
        extrudedPoint += 0.75f * (point - rbody.transform.position).normalized;
        var newDamageObject = GameObject.Instantiate(damagePrefab, extrudedPoint, Quaternion.identity, transform);
        var newDamage = newDamageObject.GetComponent<CarDamage>();
        newDamage.Init(damage, this);
        damageObjects.Add(newDamage);
    }

    private void UpdateStatsForHPBracket(HPBracket bracket)
    {
        carController.maxSpeed = bracket.maxSpeed;
    }

    private HPBracket BracketForHP(float hp)
    {
        for(var i = hpBrackets.Length-1; i >= 0; i--)
        {
            if (hp >= hpBrackets[i].hp)
                return hpBrackets[i];
        }

        return hpBrackets[0];
    }

    private void ExplodeAtPoint(Vector3 point)
    {
        Debug.Log("Exploding at point " + point);
        // When the explosion happens, we generally want the
        // rbody.AddExplosionForce(outOfHPExplosionForce, point + 2*Vector3.down, 10f);
        var forceVector = outOfHPExplosionForce*((rbody.position - point).normalized + Vector3.up).normalized;
        rbody.AddForceAtPosition(forceVector, point, ForceMode.Impulse);
    }
}
