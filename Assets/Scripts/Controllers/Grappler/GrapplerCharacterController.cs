using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerCharacterController : MonoBehaviour {

    private enum State
    {
        Grappling,
        InStation,
        SlidingToPosition
    }

    public Rigidbody car;
    public Camera cam;

    public Vector3[] movementPointsRelativeToCar;
    public float movementSpeed;

    public float stationEntranceSpeed = 2f;
    public float stationUseDistance = 0.2f;

    public GrapplerInput input;

    private Vector3 forwardRelativeToCar;
    private Vector3 upwardRelativeToCar;

    [HideInInspector]
    public float movementPointPosition = 0f;
    [HideInInspector]
    public CarAttachment selectedAttachment { get; private set; }
    private State state = State.Grappling;
    private Vector3 positionRelativeToCar;

    private Vector3 slidingDestinationRelativeToCar;
    private State slidingDestinationState;

    void Awake()
    {
        selectedAttachment = null;
    }

    // Use this for initialization
    void Start () {
        forwardRelativeToCar = car.transform.InverseTransformDirection(transform.forward);
        upwardRelativeToCar = car.transform.InverseTransformDirection(transform.up);
    }
	
	// Update is called once per frame
	void Update () {
        if(state == State.InStation)
        {
            UpdateMovementInStation();
            UpdateAttachmentUseInStation();

        } else if(state == State.SlidingToPosition)
        {
            UpdateSlidingMovement();
        } else
        {
            UpdateMovement();
            UpdateAttachmentUse();
        }

        positionRelativeToCar = car.transform.InverseTransformPoint(transform.position);
    }

    Vector3 GetCarRelativePositionForMovementPointPosition(float position)
    {
        // Iterate through the movement points
        var remainingDistance = position % GetMovementPointMaxDistance();
        for(var i = 0; i < movementPointsRelativeToCar.Length; i++)
        {
            var currentPoint = movementPointsRelativeToCar[i];
            var nextPoint = movementPointsRelativeToCar[(i + 1) % movementPointsRelativeToCar.Length];
            var distance = Vector3.Distance(currentPoint, nextPoint);
            if (remainingDistance <= distance)
            {
                return Vector3.Lerp(currentPoint, nextPoint, remainingDistance / distance);
            } else
            {
                remainingDistance -= distance;
            }
        }

        Debug.LogError("There was a problem getting the movement position");
        return transform.position;
    }
    
    CarAttachment GetAttachmentToUse()
    {
        return input.GetAttachmentToUse();
    }

    void UpdateAttachmentUse()
    {
        var newAttachment = GetAttachmentToUse();
        if(selectedAttachment != null && newAttachment != selectedAttachment)
        {
            selectedAttachment.Unhighlight();
        }

        selectedAttachment = newAttachment;
        if(selectedAttachment != null)
        {
            selectedAttachment.Highlight();

            if(input.GetUseButtonDown() && selectedAttachment.IsUseable())
            {
                selectedAttachment.Use(this);
            }
            if(input.GetUseButtonUp() && selectedAttachment.beingUsed)
            {
                selectedAttachment.EndUseManual();
            }
        }
    }

    void UpdateAttachmentUseInStation()
    {
        if (input.GetUseButtonDown() && selectedAttachment.IsUseable())
        {
            selectedAttachment.Use(this);
        }
        if (input.GetUseButtonUp() && selectedAttachment.beingUsed)
        {
            selectedAttachment.EndUseManual();
        }

        selectedAttachment.transform.rotation = cam.transform.rotation;
    }

    void UpdateMovement()
    {
        var movement = movementSpeed * -input.GetHorizontalInput();
        movementPointPosition += movement * Time.deltaTime;
        if (movementPointPosition < 0)
        {
            movementPointPosition += GetMovementPointMaxDistance();
        } else if(movementPointPosition > GetMovementPointMaxDistance())
        {
            movementPointPosition -= GetMovementPointMaxDistance();
        }
        var positionRelativeToCar = GetCarRelativePositionForMovementPointPosition(movementPointPosition);

        transform.position = car.transform.TransformPoint(positionRelativeToCar);

        UpdateLookDirection();
    }

    void UpdateSlidingMovement()
    {
        if(SlidingReachedDestination())
        {
            transform.position = car.transform.TransformPoint(slidingDestinationRelativeToCar);
            state = slidingDestinationState;
        } else
        {
            var currentPosition = positionRelativeToCar;
            transform.position = car.transform.TransformPoint(Vector3.MoveTowards(currentPosition, slidingDestinationRelativeToCar, stationEntranceSpeed * Time.deltaTime));
        }
    }

    void UpdateLookDirection()
    {
        var forwardLookDir = -positionRelativeToCar.normalized;
        forwardLookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(car.transform.TransformDirection(forwardLookDir), car.transform.TransformDirection(upwardRelativeToCar));
    }

    void UpdateMovementInStation()
    {
        transform.position = selectedAttachment.transform.position;

        // Exit the station if the player is trying to move
        var inputVector = new Vector3(input.GetHorizontalInput(), 0, input.GetVerticalInput());
        if(inputVector.magnitude > 0.5f)
        {
            var movementDirectionCarSpace = Quaternion.FromToRotation(car.transform.forward, cam.transform.forward) * inputVector.normalized;
            var outputPos = CastAgainstCarMovementPoints(car.transform.InverseTransformPoint(transform.position), movementDirectionCarSpace);
            var outputPosReal = GetCarRelativePositionForMovementPointPosition(outputPos);

            state = State.SlidingToPosition;
            slidingDestinationState = State.Grappling;
            slidingDestinationRelativeToCar = outputPosReal;
            movementPointPosition = outputPos;

            // If this is a player, free their looking
            if (input is GrapplerPlayerInput)
            {
                var castInput = (GrapplerPlayerInput)input;
                castInput.controllerLook.clampHorizontalLook = true;
            }
        }

        UpdateLookDirection();
    }

    public void EnterStation(CarAttachment station)
    {
        state = State.SlidingToPosition;
        slidingDestinationState = State.InStation;
        slidingDestinationRelativeToCar = car.transform.InverseTransformPoint(station.transform.position);
        selectedAttachment = station;

        // If this is a player, free their looking
        if(input is GrapplerPlayerInput)
        {
            var castInput = (GrapplerPlayerInput)input;
            castInput.controllerLook.clampHorizontalLook = false;
        }

        BroadcastMessage("DidEnterStation", station, SendMessageOptions.DontRequireReceiver);
    }

    bool SlidingReachedDestination()
    {
        return Vector3.Distance(slidingDestinationRelativeToCar, car.transform.InverseTransformPoint(transform.position)) < stationUseDistance;
    }


    // Movement Utilities

    // This function determines a point along the path created by the car's movement points that intersects with a ray cast from starting position in direction. 
    // In this case, all vectors are in car-space and projected onto the car's steering plane
    public float CastAgainstCarMovementPoints(Vector3 startingPosition, Vector3 direction)
    {
        var projectedStartingPos = startingPosition;
        projectedStartingPos.y = 0;
        var projectedDirection = direction;
        projectedDirection.y = 0;

        var ray = new Ray(projectedStartingPos, projectedDirection);
        var distanceTotal = 0f;

        for(var i = 0; i < movementPointsRelativeToCar.Length; i++) { 
            var currentPoint = movementPointsRelativeToCar[i];
            currentPoint.y = 0;
            var nextPoint = movementPointsRelativeToCar[(i + 1) % movementPointsRelativeToCar.Length];
            nextPoint.y = 0;

            var upPoint = currentPoint + Vector3.up;

            var plane = new Plane(currentPoint, nextPoint, upPoint);
            float distance;
            if(plane.Raycast(ray, out distance))
            {
                var collisionPoint = ray.origin + distance * ray.direction.normalized;
                return distanceTotal + Vector3.Distance(currentPoint, collisionPoint);
            }

            distanceTotal += Vector3.Distance(currentPoint, nextPoint);
        }

        return 0;
    }

    public float GetClosestMovementPositionForWorldSpacePoint(Vector3 point)
    {
        var projectedStartingPos = car.transform.InverseTransformPoint(point);
        projectedStartingPos.y = 0;

        var distanceTotal = 0f;

        Vector3 closestPoint = new Vector3(Mathf.Infinity, 0, 0);
        var movementPos = 0f;

        for (var i = 0; i < movementPointsRelativeToCar.Length; i++)
        {
            var currentPoint = movementPointsRelativeToCar[i];
            currentPoint.y = 0;
            var nextPoint = movementPointsRelativeToCar[(i + 1) % movementPointsRelativeToCar.Length];
            nextPoint.y = 0;

            var upPoint = currentPoint + Vector3.up;

            var plane = new Plane(currentPoint, nextPoint, upPoint);

            var closestPointForPlane = plane.ClosestPointOnPlane(projectedStartingPos);
            if (Vector3.Distance(closestPointForPlane, point) < Vector3.Distance(closestPoint, point))
            {
                closestPoint = closestPointForPlane;
                movementPos = distanceTotal + Vector3.Distance(currentPoint, closestPoint);
            }

            distanceTotal += Vector3.Distance(currentPoint, nextPoint);
        }

        return movementPos;
    }

    public float GetMovementPointMaxDistance()
    {
        var runningTotal = 0f;
        for (var i = 0; i < movementPointsRelativeToCar.Length; i++)
        {
            runningTotal += Vector3.Distance(movementPointsRelativeToCar[i], movementPointsRelativeToCar[(i + 1) % movementPointsRelativeToCar.Length]);
        }
        return runningTotal;
    }
}
