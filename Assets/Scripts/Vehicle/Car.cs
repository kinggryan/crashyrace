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
    public SimpleCarController carController;
    public float maxHP = 100f;
    public HPBracket[] hpBrackets;

    public float outOfHPExplosionForce;

    public GameObject damagePrefab;

    float hp;
    private CarDamage[] currentDamageObject;

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

    public void ExplodeAtPoint(Vector3 point)
    {
        Debug.Log("Exploding at point " + point);
        // When the explosion happens, we generally want the
        // rbody.AddExplosionForce(outOfHPExplosionForce, point + 2*Vector3.down, 10f);
        var forceVector = outOfHPExplosionForce*((rbody.position - point).normalized + Vector3.up).normalized;
        rbody.AddForceAtPosition(forceVector, point, ForceMode.Impulse);
    }
}
