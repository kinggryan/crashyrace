using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverCameraController : MonoBehaviour {

    public Rigidbody car;

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
        // Determine where to move to
        // var carDirection = Vector3.ProjectOnPlane(car.velocity, Vector3.up).normalized;
        // var carFacingDirection = Vector3.ProjectOnPlane(car.transform.forward, Vector3.up).normalized;
        // var angleDifference = Vector3.SignedAngle(carFacingDirection, Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up);
        // Debug.Log(angleDifference);
        // if (Mathf.Abs(angleDifference) > maxDifferenceBetweenForwardAndCamera)
        // {
        //     carDirection = Quaternion.AngleAxis(maxDifferenceBetweenForwardAndCamera * -Mathf.Sign(angleDifference), Vector3.up) * carFacingDirection;
        //     //carDirection = Vector3.RotateTowards(carDirection, carFacingDirection, Mathf.Deg2Rad*angleDifference, Mathf.Infinity);
        // }
        //// carDirection = car.transform.InverseTransformDirection(carDirection);
        // var targetPositionRelativeToCar =  -followDistance * carDirection + followHeight * transform.up;
        // var currentPositionRelativeToCar = transform.position - car.position;
        // var newPositionRelativeToCar = Vector3.RotateTowards(currentPositionRelativeToCar, targetPositionRelativeToCar, Mathf.Infinity, Mathf.Infinity);
        // transform.position = car.position + newPositionRelativeToCar;

        var targetPositionRelativeToCar = -followDistance * car.transform.forward + followHeight * transform.up;
        var currentPositionRelativeToCar = transform.position - car.position;
        var newPositionRelativeToCar = Vector3.RotateTowards(currentPositionRelativeToCar, targetPositionRelativeToCar, Mathf.Infinity, Mathf.Infinity);
        transform.position = car.position + newPositionRelativeToCar;

        // There are two target locations we want to get to
        // If either of them is too far away, we should go to that one
        // Try to hit the position based on velocity
        // But pull it towards the forward facing direction if they mismatch by a ton

        //var targetPositionProjection = Vector3.ProjectOnPlane(targetPosition, Vector3.up);
        //var currentPositionProjection = Vector3.ProjectOnPlane(transform.position, Vector3.up);
        //var rotationAngle = Vector3.Angle(currentPositionProjection, targetPositionProjection);
        //Debug.Log("angle " + rotationAngle);
        //transform.RotateAround(car.position, Vector3.up, rotationAngle);

        // Move there
        //  var newPositionCarSpace = Vector3.RotateTowards(currentPositionCarSpace, targetPositionCarSpace, Mathf.Infinity, lerpRate * Time.deltaTime);
        //   transform.position = car.transform.TransformPoint(newPositionCarSpace);

        // Look at the target looky position
        // var targetLookPosition = car.position + lookAheadDistance * carDirection;
        var lookPosition = car.position + lookAheadDistance * car.transform.forward; // Vector3.Lerp(previousLookPosition, targetLookPosition, lerpRate * Time.deltaTime);
        transform.LookAt(lookPosition);
     //   previousLookPosition = lookPosition;
    }

    

    float FollowDistanceFromCarSpeed(float speed)
    {
        return 0;
    }
}
