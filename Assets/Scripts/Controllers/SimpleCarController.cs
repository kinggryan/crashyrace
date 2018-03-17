using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleCarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxBrakeTorque; // Maximum torque the motor can apply when braking
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public float maxSpeed;

    private float previousMotor;
    private new Rigidbody rigidbody;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Start()
    {
        foreach(var axleInfo in axleInfos)
        {
            axleInfo.leftWheel.ConfigureVehicleSubsteps(5, 12, 15);
            axleInfo.rightWheel.ConfigureVehicleSubsteps(5, 12, 15);
        }
    }

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float brake = maxBrakeTorque * (Input.GetButton("Brake") ? 1 : 0) ;
        
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                // If we're going too fast, determine if the motor would increase our velocity in the direction of our current velocity
                // If so, don't power the wheels
                var currentVelocity = rigidbody.velocity;
                Debug.Log(Vector3.Dot(currentVelocity, transform.forward));
                if (currentVelocity.magnitude < maxSpeed || Vector3.Dot(currentVelocity, transform.forward) < 0)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                } else
                {
                    axleInfo.leftWheel.motorTorque = 0;
                    axleInfo.rightWheel.motorTorque = 0;
                }
            }
            axleInfo.leftWheel.brakeTorque = brake;
            axleInfo.rightWheel.brakeTorque = brake;
        }

        previousMotor = motor;
    }

    public bool IsGrounded()
    {
        // Considered grounded if one full axle is grounded
        foreach(var axle in axleInfos)
        {
            if(axle.rightWheel.isGrounded && axle.leftWheel.isGrounded)
            {
                return true;
            }
        }

        return false;
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
