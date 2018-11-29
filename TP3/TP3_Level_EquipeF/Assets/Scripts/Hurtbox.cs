using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {

    [SerializeField]
    [Range(1, 100)]
    int Damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth health = collision.collider.GetComponent<PlayerHealth>();
        PlayerControler player = collision.collider.GetComponent<PlayerControler>();
        if (health && player)
        {
            //vector from self-transform to target-transform
            Vector3 direction = collision.transform.position - gameObject.transform.position;
            //Hurt the player & send them flying back a bit
            health.Hurt(Damage);
            player.Knockback(direction);
        }
    }

    //Just in case
    private void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }
}
