using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ICarControlInput: MonoBehaviour {

    public abstract float GetMotorInput();
    public abstract float GetSteerInput();
    public abstract float GetBrakeInput();
}
