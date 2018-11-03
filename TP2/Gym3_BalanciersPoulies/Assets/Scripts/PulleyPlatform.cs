using UnityEngine;
using System.Collections;

public class PulleyPlatform : MonoBehaviour {

    [SerializeField]
    GameObject[] linkedPlatforms;
    public float speed = 1;
    
    [SerializeField]
    bool isUp = true;

    bool isMoving = false;
    float currentMovement = 0;
    float length;
    public Vector3 upmostPosition, downmostPosition;


    void Start () {
        length = Vector3.Distance(upmostPosition, downmostPosition);

        //initialise the platform to a position
        if (isUp) transform.position = upmostPosition;
        else transform.position = downmostPosition;
    }

    void Update()
    {
        currentMovement = Time.deltaTime * speed / length;

        if (isMoving)
        {
            Move(true);
        }
    }

    void LinkMove(LinkedMovement linkedMovement)
    { 
        isUp = linkedMovement.isUp;
        currentMovement = linkedMovement.movement;
        Move(false);
    }

    void SendInformationToLinkedPlatforms()
    {
         foreach (GameObject t in linkedPlatforms) {
            LinkedMovement informationToSend = new LinkedMovement(!isUp, currentMovement);
            { t.SendMessage("LinkMove", informationToSend, SendMessageOptions.DontRequireReceiver); }
            }
    }

    void Move(bool sendToOthers)
    {
        if (isUp && transform.position != upmostPosition)
        {
            MoveUp();
        } else if (transform.position != downmostPosition)
        {
            MoveDown();
        }
        if (sendToOthers)
        {
            SendInformationToLinkedPlatforms();
        }
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
        isMoving = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isUp = true;
        isMoving = false;
    }
}
public class LinkedMovement
{
        public LinkedMovement(bool isUp, float movement)
        {
            this.isUp = isUp;
            this.movement = movement;
        }

        public bool isUp { get; set; }
        public float movement { get; set; }
}
