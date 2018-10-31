using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {
    
    float currentTime = 0;
    
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        currentTime = currentTime + Time.deltaTime;
    }

    public void Lit()
    {
        GetComponent<Renderer>().enabled = true;
        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.enabled = true;
        }
    }
}
