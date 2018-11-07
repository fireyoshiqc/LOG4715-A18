using UnityEngine;
using System.Collections.Generic;

public class PulleyPlatform : MonoBehaviour {

    [SerializeField]
    Transform center;
    [SerializeField]
    float speed = 5;
    [SerializeField]
    float upmostPositionOffset;
    [SerializeField]
    float downmostPositionOffset;

    Vector3 upmostPosition, downmostPosition;
    public bool isPositive = true;
    float currentMovement = 0;
    float length;
    bool isUp;

    [HideInInspector]
    public float massToApply = 0;
    [HideInInspector]
    public float massOnPlatform;

    void Start () {
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
        massOnPlatform = (isUp) ? 0 :  collision.rigidbody.mass;

        if (collision.gameObject.layer == 0 || collision.gameObject.layer == 9 || collision.gameObject.layer == 11)
            collision.transform.parent = center;
    }

     private void OnCollisionStay(Collision collision)
    {
    }

    private void OnCollisionExit(Collision collision)
    {
        massOnPlatform -= (isUp) ? 0 : collision.rigidbody.mass;
        if (massOnPlatform < 0)
            massOnPlatform = 0;
        collision.transform.parent = null;
    }
}

