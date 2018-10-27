using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public abstract State Update();
    }
    
    public GrapplerCharacterController controller;
    public Camera cam;
    public CarAttachment turretPlatform;

    private State state;

    private void Awake()
    {
        state = new GrapplerSteeringStateUseAttachment(controller, cam, turretPlatform);
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

    private void Update()
    {
        var newState = state.Update();
        if(newState != null)
        {
            state = newState;
        }
    }
}
