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

    public int carNumber;
    public Rigidbody rbody;
    public Collider carBodyCollider;
    public SimpleCarController carController;
    public float maxHP = 100f;
    [HideInInspector]
    public float hp { get; private set; }
    public HPBracket[] hpBrackets;

    public float scrap = 50f;
    public float maxScrap = 100f;

    public float outOfHPExplosionForce;

    public GameObject damagePrefab;
    public GameObject scrapPrefab;
    
    [HideInInspector]
    public List<CarDamage> damageObjects { get; private set; }
    private List<Pickup> pickups = new List<Pickup>();

    private static List<Car> allCars;

    // When the car takes this much damage, the counter resets and a scrap is spawned
    private float scrapDropDamageCounter = 10f;
    private float scrapDropDamageCounterMax = 10f;
    private float scrapDropFromOrbTimer = 2f;
    private float scrapDropFromOrbTimerMax = 2f;
    private float scrapGainFromOrbPerSecond = 1f;

	// Use this for initialization
	void Awake () {
        hp = maxHP;
        if (allCars == null)
            allCars = new List<Car>();
        allCars.Add(this);
        damageObjects = new List<CarDamage>();
        Scoreboard.SetScoreForCar(this,0);
    }

    private void Update()
    {
        if(HasOrb())
        {
            // Every 2 seconds, the car with the orb drops scrap. This functions as both a trail for other cars and as a catch-up mechanic for other cars
            scrapDropFromOrbTimer -= Time.deltaTime;
            if(scrapDropFromOrbTimer <= 0)
            {
                scrapDropFromOrbTimer += scrapDropFromOrbTimerMax;
                DropScrap();
            }

            // Gain scrap over time due to having the orb
            GainScrap(scrapGainFromOrbPerSecond * Time.deltaTime);
        }
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

        scrapDropDamageCounter -= damage;
        if(scrapDropDamageCounter <= 0)
        {
            scrapDropDamageCounter += scrapDropDamageCounterMax;
            DropScrap();
        }
    }

    public void RepairDamage(float damage)
    {
        var previousBracket = BracketForHP(hp);
        hp = Mathf.Min(maxHP, hp + damage);
        var newBracket = BracketForHP(hp);
        if (!Mathf.Approximately(newBracket.hp, previousBracket.hp))
        {
            UpdateStatsForHPBracket(newBracket);
        }
    }

    public void GainScrap(float amount)
    {
        scrap = Mathf.Min(maxScrap, scrap + amount);
    }

    // Returns true if the scrap payment was successful
    public bool PayScrap(float amount)
    {
        if (scrap < amount)
            return false;
        scrap -= amount;
        return true;
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
        if(pickup.isAttachable)
        {
            pickup.AttachToCar(this, 5 * Vector3.up);
            pickups.Add(pickup);
        } else
        {
            pickup.WasAcquiredByCar(this);
        }

        BroadcastMessage("DidAcquirePickup", pickup, SendMessageOptions.DontRequireReceiver);
    }

    public void DropPickup(Pickup pickup)
    {
        Debug.Log("Dropped");
        pickup.SetPickupEnabled(true);
        pickup.TemporarilyDisableCollisionsWith(carBodyCollider);
        pickup.RemoveFromCar();
        pickups.Remove(pickup);

        BroadcastMessage("DidDropPickup", pickup, SendMessageOptions.DontRequireReceiver);
    }

    public void DropScrap()
    {
        var newScrapGameObject = GameObject.Instantiate(scrapPrefab, transform.position, Quaternion.identity);
        var newScrap = newScrapGameObject.GetComponent<ScrapPickup>();
        newScrap.TemporarilyDisableCollisionsWith(carBodyCollider);
    }

    public void EnteredWaypoint(Waypoint waypoint)
    {
        if(HasOrb())
        {
            Scoreboard.ScoreForCar(this);
            // TODO: Disable this waypoint for yourself for like 30 seconds or longer
        }

        waypoint.DisableForCar(this);
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

    public bool HasOrb()
    {
        foreach(var pickup in pickups)
        {
            if(pickup is OrbPickup)
            {
                return true;
            }
        }

        return false;
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
