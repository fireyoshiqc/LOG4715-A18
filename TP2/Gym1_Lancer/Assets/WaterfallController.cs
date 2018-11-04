using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterfallController : MonoBehaviour {

  public float flameReductionStrength = 1.0f;
  private BasicTorchController torch;

  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (torch && torch.flameLife > 0.0f)
    {
      torch.flameLife -= flameReductionStrength * Time.deltaTime;
    }
	}

  private void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.tag == "torch")
    {
      if (!torch)
        torch = col.gameObject.GetComponent<BasicTorchController>();
    }
  }

  private void OnTriggerStay(Collider col)
  {
    if (col.gameObject.tag == "torch")
    {
      if (!torch)
        torch = col.gameObject.GetComponent<BasicTorchController>();
    }
  }

  private void OnTriggerExit(Collider col)
  {
    torch = null;
  }
}
