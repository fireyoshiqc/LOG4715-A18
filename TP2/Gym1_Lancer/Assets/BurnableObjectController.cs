using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnableObjectController : MonoBehaviour
{

  public bool lit = false;
  public float burnTime = 10.0f;
  public int lightSmoothingFactor = 10; // Smoothness of light flickering
  public float lightIntensity = 5.0f;
  private Color startColor;
  private Color delta;
  private Material burnShader;
  private float initialBurnTime;
  private BasicTorchController torch;
  private ParticleSystem.EmissionModule flameEmission;
  private ParticleSystem.EmissionModule ashEmission;
  private Light[] lights;
  private float[] lightSmoothing;

  // Use this for initialization
  void Start()
  {
    startColor = GetComponent<Renderer>().materials[0].color;
    initialBurnTime = burnTime;
    delta = startColor / burnTime;
    if (GetComponent<Renderer>().materials.Length > 1)
    {
      burnShader = GetComponent<Renderer>().materials[1];
      burnShader.SetFloat("_DissolveAmount", 1.0f);
    }
    flameEmission = GetComponentsInChildren<ParticleSystem>()[0].emission;
    flameEmission.enabled = false;
    ashEmission = GetComponentsInChildren<ParticleSystem>()[1].emission;
    ashEmission.enabled = false;
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
      if (lit)
      {
        GetComponent<Renderer>().enabled = false;
        foreach (Collider collider in GetComponents<Collider>())
        {
          collider.enabled = false;
        }
        ashEmission.enabled = true;
        GetComponentsInChildren<ParticleSystem>()[1].Play();
        Destroy(gameObject, 2.0f);
        this.enabled = false;
      }
    }
  }

  private void OnTriggerEnter(Collider col)
  {
    if (lit) return;
    if (col.gameObject.tag == "torch")
    {
      if (!torch)
        torch = col.gameObject.GetComponent<BasicTorchController>();

      if (torch.flameLife > 0.0f)
      {
        lit = true;
        flameEmission.enabled = true;
      }
    }
  }

  private void OnTriggerStay(Collider col)
  {
    if (lit) return;
    if (col.gameObject.tag == "torch")
    {
      if (!torch)
        torch = col.gameObject.GetComponent<BasicTorchController>();

      if (torch.flameLife > 0.0f)
      {
        lit = true;
        flameEmission.enabled = true;
      }
    }
  }

  private void OnTriggerExit(Collider col)
  {

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

  private void CreateAshes()
  {
  }

  /*
  void LightItUp()
  {
    GetComponentInChildren<ParticleSystem>().Play();
    Light[] lights = GetComponentsInChildren<Light>();
    foreach (Light light in lights)
    {
      light.enabled = true;
    }
    player.SpawnPos.y = transform.position.y;
    player.SpawnPos.z = transform.position.z;
    if (LinkedDoor)
      LinkedDoor.InteractedUpdate(true);
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
  */
}
