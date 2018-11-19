using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrasierController : MonoBehaviour {

    private GameObject torch;
    public bool lit = false;
    public PlayerControler player;
    public Platform_Mover LinkedDoor;

	// Use this for initialization
	void Start () {
        if (!lit)
        {
            PutOut();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "torch")
        {
            if (!torch)
                torch = col.gameObject;
            FlameController flame = torch.GetComponent<FlameController>();
            flame.flameLife = flame.maxFlameLife;

            if (!lit)
                LightItUp();
        }
    }

    void OnTriggerExit(Collider col)
    {
        
    }

    void LightItUp()
    {
        lit = true;
        GetComponentInChildren<ParticleSystem>().Play();
        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.enabled = true;
            Collider c = light.GetComponent<Collider>();
            if (c != null) { c.enabled = true; }
        }
        player.SpawnPos.y = transform.position.y;
        player.SpawnPos.z = transform.position.z;
        if(LinkedDoor)
            LinkedDoor.InteractedUpdate(true);
    }

    void PutOut()
    {
        lit = false;
        GetComponentInChildren<ParticleSystem>().Stop();
        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.enabled = false;
            Collider c = light.GetComponent<Collider>();
            if (c != null) { c.enabled = false; }
        }
    }
}
