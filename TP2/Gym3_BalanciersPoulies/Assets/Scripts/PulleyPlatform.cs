using UnityEngine;
using System.Collections.Generic;

public class PulleyPlatform : MonoBehaviour {

    [SerializeField]
    float speed = 5;
    [SerializeField]
    float upmostPositionOffset = 0;
    [SerializeField]
    float downmostPositionOffset = 0;

    Vector3 upmostPosition, downmostPosition;
    private Rigidbody rb;
    public bool isPositive = true;
    float currentMovement = 0;
    float length;

    [HideInInspector]
    public float massToApply = 0;
    [HideInInspector]
    public float massOnPlatform;



    void Start () {
        ////initialise the platform to a position
        //if (isUp) transform.position = upmostPosition;
        //else transform.position = downmostPosition;
        rb = gameObject.GetComponent<Rigidbody>();

        upmostPosition = transform.position;
        upmostPosition.y += upmostPositionOffset;

        downmostPosition = transform.position;
        downmostPosition.y -= downmostPositionOffset;

        length = Vector3.Distance(upmostPosition, downmostPosition);
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
         //rb.MovePosition(Vector3.Lerp(transform.position, upmostPosition, currentMovement));
    }

    void MoveDown()
    {
        transform.position = Vector3.Lerp(transform.position, downmostPosition, currentMovement);  
        //rb.MovePosition(Vector3.Lerp(transform.position, downmostPosition, currentMovement)); 
    }
        
    void OnCollisionEnter(Collision collision)
    {
        bool isUp = (transform.position.y - collision.transform.position.y > 0);
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

