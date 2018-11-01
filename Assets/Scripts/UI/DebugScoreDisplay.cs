using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScoreDisplay : MonoBehaviour {

    public UnityEngine.UI.Text displayText;

    private bool someoneHasWon = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (someoneHasWon)
            return;

        var text = "";
        foreach(var carScore in Scoreboard.points)
        {
            text += carScore.Key.carNumber + ": " + carScore.Value + " | ";
        }
        displayText.text = text;
	}

    public void CarWon(Car car)
    {
        displayText.text = car.carNumber + " HAS WON!!!";
        someoneHasWon = true;
    }
}
