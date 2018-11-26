using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrasierController : MonoBehaviour {

    private GameObject torch;
    public bool lit = false;
    public PlayerControler player;
    public Platform_Mover LinkedDoor;

    public RespawnController Resetter;

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
            if (flame.flameLife > 0.0f && !lit)
            {
                LightItUp();
                Resetter.TriggerCheckpoint(torch);
            }
            else if (lit)
            {
                flame.flameLife = flame.maxFlameLife;
            }
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
        }
        player.SpawnPos.y = transform.position.y;
        player.SpawnPos.x = transform.position.x;
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
        }
    }
}
