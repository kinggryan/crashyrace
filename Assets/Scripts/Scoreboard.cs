using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Scoreboard {

    public static Dictionary<Car, int> points = new Dictionary<Car, int>();

    const int victoryPoints = 5;

	// Use this for initialization
    public static void SetScoreForCar(Car car, int score)
    {
        points[car] = score;
    }

	public static void ScoreForCar(Car car)
    {
        if(points.ContainsKey(car))
        {
            points[car]++;
            if(points[car] >= victoryPoints)
            {
                Object.FindObjectOfType<DebugScoreDisplay>().CarWon(car);
            }
        } else
        {
            points[car] = 1;
        }
    }
}
