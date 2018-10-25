using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverDecisionLayer : MonoBehaviour {

    public abstract class State
    {
        protected DriverSteeringLayer steeringLayer;
        protected DriverDecisionLayer decisionLayer;

        public State(DriverSteeringLayer steeringLayer, DriverDecisionLayer decisionLayer)
        {
            this.steeringLayer = steeringLayer;
            this.decisionLayer = decisionLayer;
        }
        public abstract State Update();
        public virtual State DidAcquirePickup(Pickup pickup)
        {
            return null;
        }
        public virtual State DidDropPickup(Pickup pickup)
        {
            return null;
        }
    }

    public DriverSteeringLayer steeringLayer;

    private State state;

    private void Awake()
    {
        // On awake, pursue the orb
        state = new DriverDecisionStatePursuit(steeringLayer, this, Object.FindObjectOfType<OrbPickup>().transform);
    }

    private void Update()
    {
        var newState = state.Update();
        if (newState != null)
            state = newState;
    }

    private void DidAcquirePickup(Pickup pickup)
    {
        var newState = state.DidAcquirePickup(pickup);
        if (newState != null)
        {
            state = newState;
            Debug.Log("Transitioned to state: " + newState);
        }
    }

    private void DidDropPickup(Pickup pickup)
    {
        var newState = state.DidDropPickup(pickup);
        if (newState != null)
        {
            state = newState;
            Debug.Log("Transitioned to state: " + newState);
        }
    }
}
