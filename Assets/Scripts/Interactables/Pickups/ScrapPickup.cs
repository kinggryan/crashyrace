using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPickup : Pickup {

    public float scrapGain = 5f;

    // Use this for initialization
    public override void WasAcquiredByCar(Car car)
    {
        car.GainScrap(scrapGain);
        GameObject.Destroy(gameObject);
    }
}
