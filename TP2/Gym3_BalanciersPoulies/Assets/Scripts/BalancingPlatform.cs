using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BalancingPlatform : MonoBehaviour {

    [SerializeField]
    float minimumX = -40;
    [SerializeField]
    float maximumX = 40;
    [SerializeField]
    float speed = 1f;
    float rotationX = 0F;
    private List<float> rotArrayX = new List<float>();
	float rotAverageX = 0F;	
    public float frameCounter = 20;

    bool isClockwise, isMoving = false;
    float startTime;
    float delayToStop = 5f;
    Quaternion clockWise, counterClockWise;
    Quaternion originalRotation;

    void Start () {
        //clockWise = new Quaternion(maxRotation, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        //counterClockWise = new Quaternion(-maxRotation, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        originalRotation = transform.localRotation;
        //length = Quaternion.Angle(clockWise, counterClockWise);
    }

    void Update()
    {
        if (Time.time - startTime > delayToStop)
        {
            isMoving = false;
        }

        if (isMoving)
        {
            rotAverageX = 0f;
		    rotationX = (isClockwise)? rotationX - speed : rotationX + speed;
		    rotArrayX.Add(rotationX);
 
		    if (rotArrayX.Count >= frameCounter) {
			    rotArrayX.RemoveAt(0);
		    }
		    for(int i = 0; i < rotArrayX.Count; i++) {
			    rotAverageX += rotArrayX[i];
		    }
		    rotAverageX /= rotArrayX.Count;
 
		    rotAverageX = ClampAngle(rotationX, minimumX, maximumX);
 
		    Quaternion xQuaternion = Quaternion.AngleAxis (rotAverageX, Vector3.left);
		    //transform.localRotation = originalRotation * xQuaternion;	
            //transform.Rotate(Vector3.left, rotAverageX);
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation * xQuaternion, speed);
        }
        
        //if (isClockwise && isMoving)
        //{
        //    //transform.rotation = Quaternion.Lerp(transform.rotation, clockWise, speed);

        //    transform.Rotate(Vector3.left * Time.deltaTime * speed);
        //}
        //else if (isMoving)
        //{
        //    //transform.rotation = Quaternion.Lerp(transform.rotation, counterClockWise, speed);
        //    Vector3 rotationToApply = Vector3.right * Time.deltaTime * speed;
        //    transform.rotation.
        //    if (Mathf.Abs((rotationToApply.x + transform.rotation.x)) < maxRotation)
        //    {
        //        transform.Rotate(rotationToApply);
        //    }
        //}
    }

    void OnCollisionEnter(Collision col)
    {
        isMoving = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        startTime = Time.time;
        isClockwise = (transform.position.z - collision.transform.position.z > 0);
    }

    //void OnCollisionExit(Collision col)
    //{
    //    this.startTime = Time.time;
    //    //isMoving = false;
    //}

    float ClampAngle (float angle, float min, float max)
	{
		angle = angle % 360;
		if ((angle >= -360F) && (angle <= 360F)) {
			if (angle < -360F) {
				angle += 360F;
			}
			if (angle > 360F) {
				angle -= 360F;
			}			
		}
		return Mathf.Clamp (angle, min, max);
	}
}