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

    private Vector3 forwardRelativeToCar;
    private Vector3 upwardRelativeToCar;

    private float movementPointPosition = 0f;

    private CarAttachment selectedAttachment = null;

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

        var movement = movementSpeed * -Input.GetAxis("Horizontal");
        movementPointPosition += movement * Time.deltaTime;
        if(movementPointPosition < 0)
        {
            movementPointPosition += GetMovementPointMaxDistance();
        }
        var positionRelativeToCar = GetCarRelativePositionForMovementPointPosition(movementPointPosition);

        transform.position = car.transform.TransformPoint(positionRelativeToCar);

        transform.rotation = Quaternion.LookRotation(car.transform.TransformDirection(forwardRelativeToCar), car.transform.TransformDirection(upwardRelativeToCar));
        mouseLook.LookRotation(transform, cam.transform);

        UpdateAttachmentUse();
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
                selectedAttachment.Use();
            }
        }
    }
}
