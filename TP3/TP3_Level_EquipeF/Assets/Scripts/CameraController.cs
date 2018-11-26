using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    public float damping = 1;
    public float offsetY = 0.0f;
    public float lookAheadFactor = 3;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;

    float offsetZ;
    Vector3 lastTargetPosition;
    Vector3 currentVelocity;
    Vector3 lookAheadPos;
    Vector3 newPos;
    Vector3 gfxVelocity;

    // Use this for initialization
    void Start()
    {
        lastTargetPosition = target.position;
        offsetZ = (transform.position - target.position).z;
        transform.parent = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // only update lookahead pos if accelerating or changed direction
        float xMoveDelta = (target.position - lastTargetPosition).x;

        bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

        if (updateLookAheadTarget)
        {
            lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
        }
        else
        {
            lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.fixedDeltaTime * lookAheadReturnSpeed);
        }

        Vector3 aheadTargetPos = target.position + Vector3.up * offsetY + lookAheadPos + Vector3.forward * offsetZ;
        newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref currentVelocity, damping);

        transform.position = newPos;

        lastTargetPosition = target.position;
    }
}
