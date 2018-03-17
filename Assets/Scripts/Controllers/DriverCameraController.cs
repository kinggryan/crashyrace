using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverCameraController : MonoBehaviour {

    public Rigidbody car;
    public SimpleCarController carController;

    float followDistance = 10f;
    float followHeight = 4f;
    float lookAheadDistance = 25f;
    float lerpRate = 15f;

    float maxDifferenceBetweenForwardAndCamera = 30f;

    Vector3 previousLookPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (carController.IsGrounded())
            PositionCameraForGroundedCar();
        else
            PositionCameraForAirborneCar();
    }
    
    void PositionCameraForGroundedCar()
    {
        //var targetPositionRelativeToCar = -followDistance * car.transform.forward + followHeight * transform.up;
        //var currentPositionRelativeToCar = transform.position - car.position;
        //var newPositionRelativeToCar = Vector3.RotateTowards(currentPositionRelativeToCar, targetPositionRelativeToCar, Mathf.Infinity, Mathf.Infinity);
        //transform.position = car.position + newPositionRelativeToCar;
        transform.position = car.position - followDistance * Vector3.ProjectOnPlane(car.transform.forward, Vector3.up).normalized + followHeight * Vector3.up;

        SetLookDirection();
    }

    void PositionCameraForAirborneCar()
    {
        var groundDirectionFromCarToPlayer = Vector3.ProjectOnPlane(transform.position - car.position, Vector3.up).normalized;
        transform.position = car.position + followDistance * groundDirectionFromCarToPlayer + followHeight * Vector3.up;

        SetLookDirection();
    }

    void SetLookDirection()
    {
        var cameraToCarGroundDirection = Vector3.ProjectOnPlane(car.position - transform.position, Vector3.up).normalized;
        var cameraToCarGroundDistance = Vector3.ProjectOnPlane(car.position - transform.position, Vector3.up).magnitude;
        var lookAtPoint = transform.position - followHeight * Vector3.up + (cameraToCarGroundDistance + lookAheadDistance) * cameraToCarGroundDirection;
        transform.LookAt(lookAtPoint);
    }

    float FollowDistanceFromCarSpeed(float speed)
    {
        return 0;
    }
}
