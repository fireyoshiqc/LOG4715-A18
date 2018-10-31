using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {

    [SerializeField]
    int Damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnCollisionEnter(Collision collision)
    {
        //Hurt the player & send them flying back a bit
        collision.gameObject.SendMessage("Hurt", Damage, SendMessageOptions.DontRequireReceiver);
        collision.gameObject.SendMessage("Knockback", null, SendMessageOptions.DontRequireReceiver);
    }

    //Just in case
    private void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }
}
