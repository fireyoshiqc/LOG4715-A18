using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtzone : MonoBehaviour {

    [SerializeField]
    LayerMask ImmunityTriggers;

    [SerializeField]
    [Range(0.1f, 100.0f)]
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
        if (!other.CompareTag("Player")) return; //We dont care about touching other objects
        //If player position overlaps the Light radius, do not trigger damage!
        Collider[] colliders = Physics.OverlapSphere(other.transform.position, 0.01f, ImmunityTriggers);
        if (colliders.Length > 0)
        {
            timeSinceEnter = 0f;
        }
        else
        {
            if (timeSinceEnter > 1.0f / DamagePerSecond)
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health)
                {
                    float oldMercy = health.MercyInvulnerability;
                    health.MercyInvulnerability = 0.0f;
                    health.Hurt(1);
                    health.MercyInvulnerability = oldMercy;
                }
                timeSinceEnter = 0f;
            }
            timeSinceEnter += Time.deltaTime;
        }
    }
}
