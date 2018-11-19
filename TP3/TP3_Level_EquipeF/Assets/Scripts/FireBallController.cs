using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour {

    public LayerMask layer;
    public int maxTravelDistance = 50;
    private Vector3 startPosition;


	// Use this for initialization
	void Start () {
        startPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter(Collision coll)
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "water")
        {
            Destroy(gameObject);
        }
    }
}
