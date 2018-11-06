using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BalancingPlatform : MonoBehaviour {

    [SerializeField]
    float mimimumAngle = -40;
    [SerializeField]
    float maximumAngle = 40;
    [SerializeField]
    float speed = 1f;

    float length = 0;
    bool isClockwise, isMoving = false;
    Quaternion minAngleQuat, maxAngleQuat;

    void Start () {
        minAngleQuat = Quaternion.AngleAxis(mimimumAngle, Vector3.left);
        maxAngleQuat = Quaternion.AngleAxis(maximumAngle, Vector3.left);

        length = Quaternion.Angle(minAngleQuat, maxAngleQuat);
    }

    void Update()
    {
        if (isMoving)
        {
            float t = Time.deltaTime * speed * 10 / length;

            if (isClockwise)
                transform.rotation = Quaternion.Slerp(transform.rotation, maxAngleQuat, t); 
            else
                transform.rotation = Quaternion.Slerp(transform.rotation, minAngleQuat, t); 
        }
    }

    void OnCollisionEnter(Collision col)
    {
        isMoving = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        isClockwise = (transform.position.z - collision.transform.position.z > 0);
    }

    void OnCollisionExit(Collision col)
    {
        isMoving = false;
    }
}
