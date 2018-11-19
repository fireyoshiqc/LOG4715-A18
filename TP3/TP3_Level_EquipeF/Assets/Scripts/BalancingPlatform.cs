using UnityEngine;

public class BalancingPlatform : MonoBehaviour {

    [SerializeField]
    [Range(-80, 0)]
    float mimimumAngle = -40;
    [SerializeField]
    [Range(0, 80)]
    float maximumAngle = 40;
    [SerializeField]
    [Range(0.1f, 10)]
    float speed = 1f;

    float length = 0;
    bool isClockwise, isOver, isMoving = false;
    Quaternion minAngleQuat, maxAngleQuat;

    float timeOfLastMove;

    void Start () {
        minAngleQuat = Quaternion.AngleAxis(mimimumAngle, Vector3.left);
        maxAngleQuat = Quaternion.AngleAxis(maximumAngle, Vector3.left);

        length = Quaternion.Angle(minAngleQuat, maxAngleQuat);

        timeOfLastMove = Time.time;
    }

    void Update()
    {
        if (timeOfLastMove - Time.time > 0.2)
            isMoving = false;

        
        if (isMoving)
        {
            float t = Time.deltaTime * speed * 10 / length;

            if (isClockwise)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, maxAngleQuat, t);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, minAngleQuat, t);
            }
                 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        isMoving = true;
        isOver = (collision.collider.GetType() == typeof(SphereCollider));
        timeOfLastMove = Time.time;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (transform.position.z - collision.transform.position.z > 0)
        {
            isClockwise = (isOver) ? true : false;
        }
        else
        {
            isClockwise = (isOver) ? false : true;
        }
        timeOfLastMove = Time.time;
    }

    void OnCollisionExit(Collision col)
    {
    }
}
