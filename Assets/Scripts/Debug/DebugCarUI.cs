using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCarUI : MonoBehaviour {

    public UnityEngine.UI.Text speedometer;
    public UnityEngine.UI.Text rpmometer;
    public SimpleCarController carController;
    public Rigidbody carBody;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        speedometer.text = "" + carBody.velocity.magnitude;
        rpmometer.text = "" + carController.axleInfos[0].leftWheel.rpm;

    }
}
