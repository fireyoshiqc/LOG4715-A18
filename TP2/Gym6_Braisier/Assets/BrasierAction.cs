using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrasierAction : MonoBehaviour {

    public GameObject torch;
    public bool lit = false; 

	// Use this for initialization
	void Start () {
        PutOut();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (lit) return;
        if (col.gameObject.tag == "torch")
        {
            if (!torch)
                torch = col.gameObject;

            lit = true;
            LightItUp();
        }


    }

    void OnTriggerExit(Collider col)
    {
        
    }

    void LightItUp()
    {
        GetComponentInChildren<ParticleSystem>().Play();
        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.enabled = true;
        }
    }

    void PutOut()
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }
}
