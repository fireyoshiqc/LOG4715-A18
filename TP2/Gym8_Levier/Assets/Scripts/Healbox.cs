using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healbox : MonoBehaviour {

    [SerializeField]
    [Range(0, 10)]
    float HealthPerSecond;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionStay(Collision collision)
    {
        
    }
}
