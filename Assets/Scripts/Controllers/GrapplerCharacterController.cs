using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerCharacterController : MonoBehaviour {

    public Rigidbody car;
    public MouseLook mouseLook;
    public Camera cam;

    public Vector3[] movementPointsRelativeToCar;
    public float movementSpeed;
    public float maxUseDistance = 1f;

    public float stationEntranceSpeed = 2f;
    public float stationUseDistance = 0.2f;

    private Vector3 forwardRelativeToCar;
    private Vector3 upwardRelativeToCar;

    private float movementPointPosition = 0f;

    private CarAttachment selectedAttachment = null;
    private bool inStation = false;
    private Vector3 positionRelativeToCar;

    // Use this for initialization
    private void Awake()
    {
        mouseLook = new MouseLook();
        mouseLook.Init(cam.transform);
    }

    void Start () {
        forwardRelativeToCar = car.transform.InverseTransformDirection(transform.forward);
        upwardRelativeToCar = car.transform.InverseTransformDirection(transform.up);
    }
	
	// Update is called once per frame
	void Update () {
        if(inStation)
        {
            UpdateMovementInStation();
            UpdateAttachmentUseInStation();

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

    float GetMovementPointMaxDistance()
    {
        var runningTotal = 0f;
        for(var i = 0; i < movementPointsRelativeToCar.Length; i++)
        {
            runningTotal += Vector3.Distance(movementPointsRelativeToCar[i], movementPointsRelativeToCar[(i + 1) % movementPointsRelativeToCar.Length]);
        }
        return runningTotal;
    }

    CarAttachment GetAttachmentToUse()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, maxUseDistance, LayerMask.NameToLayer("attachments")))
        {
            var attachment = hitInfo.collider.GetComponent<CarAttachment>();
            return attachment;
        }

        return null;
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

            if (Input.GetButtonDown("Fire1") && selectedAttachment.IsUseable())
            {
                selectedAttachment.Use(this);
            }
            if(Input.GetButtonUp("Fire1") && selectedAttachment.beingUsed)
            {
                selectedAttachment.EndUseManual();
            }
        }
    }

    void UpdateAttachmentUseInStation()
    {
        if (Input.GetButtonDown("Fire1") && selectedAttachment.IsUseable())
        {
            selectedAttachment.Use(this);
        }
        if (Input.GetButtonUp("Fire1") && selectedAttachment.beingUsed)
        {
            selectedAttachment.EndUseManual();
        }

        selectedAttachment.transform.rotation = cam.transform.rotation;
    }

    void UpdateMovement()
    {
        var movement = movementSpeed * -Input.GetAxis("Horizontal");
        movementPointPosition += movement * Time.deltaTime;
        if (movementPointPosition < 0)
        {
            movementPointPosition += GetMovementPointMaxDistance();
        }
        var positionRelativeToCar = GetCarRelativePositionForMovementPointPosition(movementPointPosition);

        transform.position = car.transform.TransformPoint(positionRelativeToCar);

        UpdateLookDirection();
    }

    void UpdateLookDirection()
    {
        transform.rotation = Quaternion.LookRotation(car.transform.TransformDirection(forwardRelativeToCar), car.transform.TransformDirection(upwardRelativeToCar));
        mouseLook.LookRotation(transform, cam.transform);
    }

    void UpdateMovementInStation()
    {
        if (!FullyInStation())
        {
            var currentPosition = positionRelativeToCar;
            var targetPosition = car.transform.InverseTransformPoint(selectedAttachment.transform.position);
            transform.position = car.transform.TransformPoint(Vector3.MoveTowards(currentPosition, targetPosition, stationEntranceSpeed * Time.deltaTime));
        } else
        {
            transform.position = selectedAttachment.transform.position;
        }

        UpdateLookDirection();
    }

    public void EnterStation(CarAttachment station)
    {
        inStation = true;
        selectedAttachment = station;
    }

    bool FullyInStation()
    {
        var targetPosition = selectedAttachment.transform.position;
        return Vector3.Distance(targetPosition, transform.position) < stationUseDistance;
    }
}
