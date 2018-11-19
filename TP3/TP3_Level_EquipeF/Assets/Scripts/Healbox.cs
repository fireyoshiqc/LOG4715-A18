﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healbox : MonoBehaviour {

    [SerializeField]
    [Range(0, 10)]
    float HealthPerSecond = 1f;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    float timeSinceEnter = 0f;
    private void OnTriggerEnter(Collider other)
    {
        timeSinceEnter = 0f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return; //We dont care about touching other objects
        if (timeSinceEnter > 1/HealthPerSecond)
        {
            other.gameObject.SendMessage("Heal", 1, SendMessageOptions.DontRequireReceiver);
            timeSinceEnter = 0f;
        }
        timeSinceEnter += Time.deltaTime;
    }
}
