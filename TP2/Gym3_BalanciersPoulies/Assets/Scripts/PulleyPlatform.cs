using UnityEngine;
using System.Collections;

public class PulleyPlatform : MonoBehaviour {

    public Vector3 up, down;
    bool isUp, isMoving = false;
    public float speed = 5f;

    float startTime,
        length;

    void Start () {
        //initialise the platform to a position
        if (isUp) transform.position = up;
        else transform.position = down;

        length = Vector3.Distance(up, down);
    }

    void Update()
    {
        if (isUp && isMoving)
        {
            //move Down
            transform.position = Vector3.Lerp(up, down, ((Time.time - startTime) * speed) / length);
        }
        else if (isMoving)
        {
            //move Up
            transform.position = Vector3.Lerp(down, up, ((Time.time - startTime) * speed) / length);
        }
    }

    //move down
    void OnCollisionEnter(Collision col)
    {
        startTime = Time.time;
        isUp = false;
        isMoving = true;
    }

    //move up
    void OnCollisionExit(Collision col)
    {
        startTime = Time.time;
        isUp = true;
        isMoving = true;
    }
}