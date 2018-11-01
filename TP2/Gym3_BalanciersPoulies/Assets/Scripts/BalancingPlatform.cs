using UnityEngine;
using System.Collections;

public class BalancingPlatform : MonoBehaviour {

    [SerializeField]
    float rotation;
    [SerializeField]
    float speed = 5f;

    bool isClockwise, isMoving = false;
    float startTime,
        length;
    Quaternion clockWise, counterClockWise;

    void Start () {
        clockWise = new Quaternion(rotation, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        counterClockWise = new Quaternion(-rotation, transform.rotation.y, transform.rotation.z, transform.rotation.w);

        length = Quaternion.Angle(clockWise, counterClockWise);
    }

    void Update()
    {
        if (isClockwise && isMoving)
        {
            //move Down
            transform.rotation = Quaternion.Lerp(transform.rotation, clockWise, ((Time.time - startTime) * speed));
        }
        else if (isMoving)
        {
            //move Up
            transform.rotation = Quaternion.Lerp(transform.rotation, counterClockWise, ((Time.time - startTime) * speed));
        }
    }

    //move down
    void OnCollisionEnter(Collision col)
    {
        startTime = Time.time;
        isMoving = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        isClockwise = (transform.position.z - collision.transform.position.z > 0);
    }

    //move up
    void OnCollisionExit(Collision col)
    {
        isMoving = false;
    }
}