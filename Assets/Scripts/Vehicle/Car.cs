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
    [HideInInspector]
    public float hp { get; private set; }
    public HPBracket[] hpBrackets;

    public float outOfHPExplosionForce;

    public GameObject damagePrefab;
    
    [HideInInspector]
    public List<CarDamage> damageObjects { get; private set; }
    private List<Pickup> pickups = new List<Pickup>();

    private static List<Car> allCars;

	// Use this for initialization
	void Awake () {
        hp = maxHP;
        if (allCars == null)
            allCars = new List<Car>();
        allCars.Add(this);
        damageObjects = new List<CarDamage>();
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
        damageObjects.Remove(obj);
        GameObject.Destroy(obj.gameObject);
    }

    public void AcquirePickup(Pickup pickup)
    {
        // Disable the pickup's trigger
        // Place it in the center of the vehicle
        pickup.SetPickupEnabled(false);
        pickup.AttachToCar(this, 5 * Vector3.up);
        pickups.Add(pickup);

        BroadcastMessage("DidAcquirePickup", pickup, SendMessageOptions.DontRequireReceiver);
    }

    public void DropPickup(Pickup pickup)
    {
        Debug.Log("Dropped");
        pickup.SetPickupEnabled(true);
        pickup.RemoveFromCar();
        pickups.Remove(pickup);

        BroadcastMessage("DidDropPickup", pickup, SendMessageOptions.DontRequireReceiver);
    }

    public List<Car> OtherCars()
    {
        var cars = new List<Car>(allCars);
        cars.Remove(this);
        return cars;
    }

    public Car ClosestEnemyCar()
    {
        var otherCars = OtherCars();
        Car closestCar = null;
        var closestDistance = Mathf.Infinity;
        foreach(var car in otherCars)
        {
            var distance = Vector3.Distance(car.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCar = car;
            }
        }

        return closestCar;
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
