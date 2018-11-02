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
    public float handbrakeSidewaysFrictionMultiplier = 0.5f;
    public float handbrakeMaxSteeringAngle;

    public Vector3 relativeCenterOfMass;  // The rigidbody center of mass relative to the position of the car

    public ICarControlInput input;

    private float previousMotor;
    private float defaultSidewaysFriction;
    private new Rigidbody rigidbody;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        defaultSidewaysFriction = axleInfos[0].leftWheel.sidewaysFriction.stiffness;
    }

    public void Start()
    {
        rigidbody.centerOfMass = relativeCenterOfMass;
        foreach(var axleInfo in axleInfos)
        {
            axleInfo.leftWheel.ConfigureVehicleSubsteps(5, 12, 15);
            axleInfo.rightWheel.ConfigureVehicleSubsteps(5, 12, 15);
        }
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * input.GetMotorInput();
        float handbrake = input.GetBrakeInput();
        float steering = Mathf.Lerp(maxSteeringAngle, handbrakeMaxSteeringAngle, handbrake) * input.GetSteerInput();

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = axleInfo.steeringInverted ? -steering : steering;
                axleInfo.rightWheel.steerAngle = axleInfo.steeringInverted ? -steering : steering;
            }
            if (axleInfo.motor)
            {
                // If we're going too fast, determine if the motor would increase our velocity in the direction of our current velocity
                // If so, don't power the wheels
                var currentVelocity = rigidbody.velocity;
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

            // if reversing direction to previous motor, actually just hit the brakes
           // Debug.Log(axleInfo.leftWheel.rpm + " ; " + motor);
            if(Mathf.Abs(axleInfo.leftWheel.rpm) > 50 && Mathf.Abs(motor) > 0.5f*maxMotorTorque && Mathf.Sign(axleInfo.leftWheel.rpm) != Mathf.Sign(motor))
                axleInfo.leftWheel.brakeTorque = maxBrakeTorque;
            else
                axleInfo.leftWheel.brakeTorque = 0;
            if (Mathf.Abs(axleInfo.rightWheel.rpm) > 50 && Mathf.Abs(motor) > 0.5f * maxMotorTorque && Mathf.Sign(axleInfo.rightWheel.rpm) != Mathf.Sign(motor))
                axleInfo.rightWheel.brakeTorque = maxBrakeTorque;
            else
                axleInfo.rightWheel.brakeTorque = 0;

            //axleInfo.leftWheel.brakeTorque = brake;
            //axleInfo.rightWheel.brakeTorque = brake;
            var sidewaysFriction = axleInfo.leftWheel.sidewaysFriction;
            sidewaysFriction.stiffness = Mathf.Lerp(defaultSidewaysFriction, defaultSidewaysFriction * handbrakeSidewaysFrictionMultiplier, handbrake);
            axleInfo.leftWheel.sidewaysFriction = sidewaysFriction;
            sidewaysFriction = axleInfo.rightWheel.sidewaysFriction;
            sidewaysFriction.stiffness = Mathf.Lerp(defaultSidewaysFriction, defaultSidewaysFriction * handbrakeSidewaysFrictionMultiplier, handbrake);
            axleInfo.rightWheel.sidewaysFriction = sidewaysFriction;

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
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
    public bool steeringInverted;   // this wheel will turn inverted (to allow rear wheel steering)
}
