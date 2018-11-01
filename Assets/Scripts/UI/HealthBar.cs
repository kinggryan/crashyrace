using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public UnityEngine.UI.Slider healthBar;
    public Car car;

	// Update is called once per frame
	void Update () {
        healthBar.value = car.hp / car.maxHP;
	}
}
