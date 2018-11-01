using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapBar : MonoBehaviour {

    public UnityEngine.UI.Slider scrapBar;
    public Car car;

    // Update is called once per frame
    void Update()
    {
        scrapBar.value = car.scrap / car.maxScrap;
    }
}
