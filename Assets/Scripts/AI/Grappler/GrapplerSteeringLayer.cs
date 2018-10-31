using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrapplerSteeringLayerDelegate
{
    void DidCompleteAllSteeringInstructions();
}

public class GrapplerSteeringLayer : GrapplerInput
{
    // Currently the steering states the grappler may want to perform are:
    // use attachment
    // fire at enemy
    public abstract class State
    {
        protected GrapplerCharacterController controller;
        protected Camera cam;

        public State(GrapplerCharacterController controller, Camera cam)
        {
            this.controller = controller;
            this.cam = cam;
        }
        public abstract float GetHorizontalInput();
        public abstract float GetVerticalInput();
        public abstract bool GetUseButtonDown();
        public abstract bool GetUseButtonUp();
        public abstract bool Update();
        public virtual CarAttachment GetAttachmentToUse() { return null; }

        public virtual void DidEnterStation(CarAttachment station) { }
    }
    
    public GrapplerCharacterController controller;
    public Camera cam;

    private State state;
    private List<State> nextStates = new List<State>();

    [HideInInspector]
    public IGrapplerSteeringLayerDelegate del;

    private void Awake()
    {
        state = new GrapplerSteeringStateIdle(controller, cam);
    }

    public override float GetHorizontalInput()
    {
        return state.GetHorizontalInput();
    }

    public override float GetVerticalInput()
    {
        return state.GetVerticalInput();
    }

    public override bool GetUseButtonDown()
    {
        return state.GetUseButtonDown();
    }

    public override bool GetUseButtonUp()
    {
        return state.GetUseButtonUp();
    }

    public override CarAttachment GetAttachmentToUse()
    {
        return state.GetAttachmentToUse();
    }

    // Steering Layer

    public void SetInstructions(List<State> states)
    {
        state = states[0];
        states.RemoveAt(0);
        nextStates = states;
    }

    void DidEnterStation(CarAttachment station)
    {
        state.DidEnterStation(station);
    }

    private void Update()
    {
        var goToNextState = state.Update();
        if(goToNextState && nextStates.Count > 0)
        {
            state = nextStates[0];
            nextStates.RemoveAt(0);
        } else
        {
            del.DidCompleteAllSteeringInstructions();
        }
    }
}
