using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BalancingPlatform : MonoBehaviour {

    [SerializeField]
    float mimimumAngle = -40;
    [SerializeField]
    float maximumAngle = 40;
    [SerializeField]
    float speed = 50f;

	float rotationToApply = 0f;	
    float currentRotation = 0f;
    bool isClockwise, isMoving = false;

    void Start () {
    }

    void Update()
    {

        if (isMoving)
        {
		    rotationToApply = (isClockwise)? - (speed * Time.deltaTime) : + (speed * Time.deltaTime); 
            currentRotation += rotationToApply;

            if (currentRotation > mimimumAngle && currentRotation  < maximumAngle)
            {
                transform.Rotate(rotationToApply, 0, 0);
            } else
            {
                currentRotation -= rotationToApply;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        isMoving = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        isClockwise = 
            ((transform.position.z - collision.transform.position.z > 0) && (transform.position.y - collision.transform.position.y < 0));
    }

    void OnCollisionExit(Collision col)
    {
        isMoving = false;
    }
}
