using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTorchController : MonoBehaviour
{
  [Range(0.0f, float.PositiveInfinity)]
  public float flameLife = 1.0f; // Current flame life, up to max. 0 means the flame has died out.
  [Range(0.01f, float.PositiveInfinity)]
  public float maxFlameLife = 2.0f; // Also dictates the children lights intensities
  [Range(0.0f, float.PositiveInfinity)]
  public float timeBeforeRegen = 5.0f; // Time in seconds before the flame starts growing back if it's still lit
  [Range(0.0f, float.PositiveInfinity)]
  public float regenRate = 0.5f; // Amount the flame grows per second when it's regenerating
  [Range(1, 100)]
  public int lightSmoothingFactor = 10; // Smoothness of light flickering
  private ParticleSystem particles;
  private Light[] lights;
  private float[] lightSmoothing;
  private float currentTime = 0.0f;
  private ParticleSystem.MinMaxCurve initialParticleSizes;
  private ParticleSystem.MinMaxCurve initialParticleSpeed;
  private ParticleSystem.MinMaxCurve initialParticleLifetime;


  // Use this for initialization
  void Start()
  {
    particles = GetComponent<ParticleSystem>();
    if (particles == null)
    {
      Debug.LogError("No particle system found for BasicTorchController script.");
      this.enabled = false;
    }

    initialParticleSizes = particles.main.startSize;
    initialParticleSpeed = particles.main.startSpeed;
    initialParticleLifetime = particles.main.startLifetime;
    lights = GetComponentsInChildren<Light>();
    if (lights.Length == 0)
    {
      Debug.LogError("No lights found for BasicTorchController script.");
      this.enabled = false;
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
    if (flameLife > 0.0f)
    {
      if (flameLife < maxFlameLife)
      {
        if (currentTime < timeBeforeRegen)
        {
          currentTime = currentTime + Time.deltaTime;
        }
        else
        {
          flameLife += regenRate * Time.deltaTime;
        }
      }
      else
      {
        flameLife = maxFlameLife;
        currentTime = 0.0f;
      }
    }
    else
    {
      currentTime = 0;
      flameLife = 0.0f;
    }

    UpdateLights();
    UpdateParticleSystem();

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
    lightSmoothing[lightSmoothing.Length - 1] = Random.Range(flameLife * 0.9f, flameLife * 1.1f);
    sum += lightSmoothing[lightSmoothing.Length - 1];

    // Compute the average of the array and assign it to the
    // light intensity.
    foreach (Light light in lights)
    {
      light.enabled = true;
      light.intensity = sum / lightSmoothing.Length;
    }
  }

  private void UpdateParticleSystem()
  {
    var main = particles.main;

    ParticleSystem.MinMaxCurve sizeCurve;
    if (initialParticleSizes.mode == ParticleSystemCurveMode.Constant)
      sizeCurve = new ParticleSystem.MinMaxCurve(initialParticleSizes.constantMax * flameLife / maxFlameLife, initialParticleSizes.constantMax * flameLife / maxFlameLife);
    else
      sizeCurve = new ParticleSystem.MinMaxCurve(initialParticleSizes.constantMin * flameLife / maxFlameLife, initialParticleSizes.constantMax * flameLife / maxFlameLife);
    main.startSize = sizeCurve;

    ParticleSystem.MinMaxCurve speedCurve;
    if (initialParticleSpeed.mode == ParticleSystemCurveMode.Constant)
      speedCurve = new ParticleSystem.MinMaxCurve(initialParticleSpeed.constantMax * flameLife / maxFlameLife, initialParticleSpeed.constantMax * flameLife / maxFlameLife);
    else
      speedCurve = new ParticleSystem.MinMaxCurve(initialParticleSpeed.constantMin * flameLife / maxFlameLife, initialParticleSpeed.constantMax * flameLife / maxFlameLife);
    main.startSpeed = speedCurve;

    ParticleSystem.MinMaxCurve lifetimeCurve;
    if (initialParticleLifetime.mode == ParticleSystemCurveMode.Constant)
      lifetimeCurve = new ParticleSystem.MinMaxCurve(initialParticleLifetime.constantMax * flameLife / maxFlameLife, initialParticleLifetime.constantMax * flameLife / maxFlameLife);
    else
      lifetimeCurve = new ParticleSystem.MinMaxCurve(initialParticleLifetime.constantMin * flameLife / maxFlameLife, initialParticleLifetime.constantMax * flameLife / maxFlameLife);
    main.startLifetime = lifetimeCurve;
  }
}
