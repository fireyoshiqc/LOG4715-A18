using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnableObjectController : MonoBehaviour
{
    public bool startLit = false;
    private bool lit = false;
    public float burnTime = 10.0f;
    private float initialBurnTime;
    public int lightSmoothingFactor = 10; // Smoothness of light flickering
    public float lightIntensity = 5.0f;
    private Color startColor;
    private Color delta;
    private Material burnShader;
    private FlameController torch;
    private ParticleSystem.EmissionModule flameEmission;
    private ParticleSystem.EmissionModule ashEmission;
    private Light[] lights;
    private float[] lightSmoothing;

    private int originalLayer;

    // Use this for initialization
    void Start()
    {
        originalLayer = gameObject.layer;
        lit = startLit;

        startColor = GetComponent<Renderer>().materials[0].color;
        initialBurnTime = burnTime;
        delta = startColor / burnTime;
        if (GetComponent<Renderer>().materials.Length > 1)
        {
            burnShader = GetComponent<Renderer>().materials[1];
            burnShader.SetFloat("_DissolveAmount", 1.0f);
        }
        if (GetComponentsInChildren<ParticleSystem>()[0])
        {
          flameEmission = GetComponentsInChildren<ParticleSystem>()[0].emission;
          flameEmission.enabled = false;
        }
        else
        {
            this.enabled = false;
        }
          
        if (GetComponentsInChildren<ParticleSystem>()[1])
        {
            ashEmission = GetComponentsInChildren<ParticleSystem>()[1].emission;
            ashEmission.enabled = false;
        }
        else
        {
            this.enabled = false;
        }
        
        lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
        lightSmoothing = new float[lightSmoothingFactor];
        for (int i = 0; i < lightSmoothing.Length; i++)
        {
            lightSmoothing[i] = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lit && burnTime > 0.0f)
        {
            burnTime -= Time.deltaTime;
            GetComponent<Renderer>().materials[0].color -= (delta * Time.deltaTime);
            if (burnShader != null)
                burnShader.SetFloat("_DissolveAmount", (burnTime / initialBurnTime));
            flameEmission.enabled = true;
            UpdateLights();
        }
        else
        {
            flameEmission.enabled = false;
            ashEmission.enabled = false;
            foreach (Light light in lights)
            {
                light.enabled = false;
            }
            if (lit && gameObject.layer == originalLayer)
            {
                GetComponent<Renderer>().enabled = false;
                //foreach (Collider collider in GetComponents<Collider>())
                //{
                //    collider.enabled = false;
                //}
                ashEmission.enabled = true;
                GetComponentsInChildren<ParticleSystem>()[1].Play();
                StartCoroutine(StopAshes(2.0f));
                //if (GetComponent<Rigidbody>())
                //Destroy(GetComponent<Rigidbody>());
                //Destroy(gameObject, 2.0f);
                
                gameObject.layer = LayerMask.NameToLayer("Disabled");
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        HandleTrigger(col);
    }

    private void OnTriggerStay(Collider col)
    {
        HandleTrigger(col);
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "torch" || col.gameObject.tag == "lantern")
        {
            if (torch)
                torch = null;
        }
    }

    private void HandleTrigger(Collider col)
    {
        if (col.gameObject.tag == "torch" || col.gameObject.tag == "lantern" )
        {
            if (!torch || col.gameObject.GetComponent<FlameController>().flameLife > torch.flameLife)
                torch = col.gameObject.GetComponent<FlameController>();
            if (lit)
            {
                torch.flameLife = torch.maxFlameLife;
            }
            else if (torch.flameLife > 0.0f)
            {
                lit = true;
                flameEmission.enabled = true;
                AudioSource burnSound = GetComponent<AudioSource>();
                if (burnSound)
                    burnSound.Play();
            }
        }
        else if (col.gameObject.tag == "burns")
        {
            if (lit)
                col.gameObject.GetComponent<BurnableObjectController>().lit = true;
        }
        else if(col.gameObject.tag == "fireball") {
            lit = true;
            flameEmission.enabled = true;
        }
    }
    private void UpdateLights()
    {
        // https://answers.unity.com/questions/34739/how-to-make-a-light-flicker.html
        float sum = .0f;

        // Shift values in the table so that the new one is at the
        // end and the older one is deleted.
        for (int i = 1; i < lightSmoothing.Length; i++)
        {
            lightSmoothing[i - 1] = lightSmoothing[i];
            sum += lightSmoothing[i - 1];
        }

        // Add the new value at the end of the array.
        float burnLight = lightIntensity * (burnTime / initialBurnTime);
        lightSmoothing[lightSmoothing.Length - 1] = Random.Range(burnLight * 0.9f, burnLight * 1.1f);
        sum += lightSmoothing[lightSmoothing.Length - 1];

        // Compute the average of the array and assign it to the
        // light intensity.
        foreach (Light light in lights)
        {
            light.enabled = true;
            light.intensity = sum / lightSmoothing.Length;
        }
    }
    
    public void ResetObject()
    {
        GetComponent<Renderer>().enabled = true;
        gameObject.layer = originalLayer;

        GetComponent<Renderer>().materials[0].color = startColor;
        lit = startLit;
        burnTime = initialBurnTime;

        if (burnShader != null)
            burnShader.SetFloat("_DissolveAmount", 1.0f);

        if (torch != null)
            torch = null;

        flameEmission.enabled = false;
        ashEmission.enabled = false;

        foreach (Light light in lights)
            light.enabled = false;
    }
    
    IEnumerator StopAshes(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponentsInChildren<ParticleSystem>()[1].Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        ashEmission.enabled = false;
    }
}
