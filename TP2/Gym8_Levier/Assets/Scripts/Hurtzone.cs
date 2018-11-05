using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtzone : MonoBehaviour {

    [SerializeField]
    LayerMask ImmunityTriggers;

    [SerializeField]
    float DamagePerSecond = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    float timeSinceEnter = 0f;
    private void OnTriggerStay(Collider other)
    {
        //If player position overlaps the Light radius, do not trigger damage!
        Collider[] colliders = Physics.OverlapSphere(other.transform.position, 0.01f, ImmunityTriggers);
        if (colliders.Length > 0)
        {
            timeSinceEnter = 0f;
        }
        else
        {
            if (timeSinceEnter > 1 / DamagePerSecond)
            {
                other.gameObject.SendMessage("Hurt", 1, SendMessageOptions.DontRequireReceiver);
                timeSinceEnter = 0f;
            }
            timeSinceEnter += Time.deltaTime;
        }
    }
}
