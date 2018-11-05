using UnityEngine;
using System.Collections.Generic;

public class PulleyPlatform : MonoBehaviour {

    [SerializeField]
    bool isUp = true;
    [SerializeField]
    float speed = 5;

    public bool isPositive = true;

    float currentMovement = 0;
    float length;

    public Vector3 upmostPosition, downmostPosition;

    [HideInInspector]
    public float massToApply = 0;
    [HideInInspector]
    public float massOnPlatform;

    void Start () {
        length = Vector3.Distance(upmostPosition, downmostPosition);

        //initialise the platform to a position
        if (isUp) transform.position = upmostPosition;
        else transform.position = downmostPosition;
    }

    void Update()
    {
        currentMovement = Time.deltaTime * speed * Mathf.Abs(massToApply) / length;

        Move();

        //massOnPlatform = 0;
    }

    void Move()
    {
        if (goingUp() && transform.position != upmostPosition)
        {
            MoveUp();
        } else if (transform.position != downmostPosition)
        {
            MoveDown();
        }
    }

    bool goingUp()
    {
        if ((isPositive && massToApply < 0) || (!isPositive && massToApply > 0))
        {
            return true;
        }
            
        return false;
    }

    void MoveUp()
    {
        transform.position = Vector3.Lerp(transform.position, upmostPosition, currentMovement); 
    }

    void MoveDown()
    {
        transform.position = Vector3.Lerp(transform.position, downmostPosition, currentMovement);   
    }
        
    void OnCollisionEnter(Collision collision)
    {
        isUp = (transform.position.y - collision.transform.position.y > 0);
        massOnPlatform = collision.rigidbody.mass;
        //massOnPlatform = (isUp) ? -collision.rigidbody.mass : collision.rigidbody.mass;
    }

     private void OnCollisionStay(Collision collision)
    {
        //isUp = (transform.position.y - collision.transform.position.y > 0);
        //massOnPlatform += collision.rigidbody.mass;
        //massOnPlatform += (isUp) ? collision.rigidbody.mass : -collision.rigidbody.mass;
    }

    private void OnCollisionExit(Collision collision)
    {
        massOnPlatform = 0;
    }
}

