using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BrasierController : MonoBehaviour {

    private GameObject torch;
    public bool lit = false;
    public PlayerController player;
    public PlatformMover LinkedDoor;

    public bool isFinal = false;
    public bool exitRight = true;

    public RespawnController Resetter;

    private AudioSource source;
    private float initVolume;

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        if (source)
        {
            initVolume = source.volume;
        }
        else
        {
            initVolume = 0.0f;
        }
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
        if (col.gameObject.tag == "torch" || col.gameObject.tag == "lantern")
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
        if(isFinal)
        {
            StartCoroutine(Cutscene());
        }
        else
        {
            if(LinkedDoor)
                LinkedDoor.InteractedUpdate(true);
            if (source)
            {
                source.volume = initVolume;
            }
        }
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
        if (source)
        {
            source.volume = 0.0f;
        }
    }

    IEnumerator Cutscene()
    {
        //Light up, turn around
        float direction = exitRight ? 1 : -1;
        player.isCutsceneControlled = true;
        if (source)
        {
            source.volume = initVolume;
        }
        player.FlipCharacter(direction);
        //Lock camera
        var cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
        if (cam)
            cam.target = transform;
        yield return new WaitForSeconds(1);
        //Dramatic door opening
        if (LinkedDoor)
            LinkedDoor.InteractedUpdate(true);
        yield return new WaitForSeconds(3);
        //Walk into the sunset
        player.cutsceneInput = direction;
        yield return new WaitForSeconds(2);
        //Quit
        SceneManager.LoadScene(0);
    }

}
