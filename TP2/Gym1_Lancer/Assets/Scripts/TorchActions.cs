﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchActions : MonoBehaviour {

  public GameObject torch;
  public GameObject lantern;
  public GameObject currentlyHeld;
  public Transform guide;

  public float maxThrowForce = 500.0f;
  public float throwChargeRate = 350.0f;
  private float _currentThrowForce = 0.0f;

  public float targetLineLengthModifier = 3.0f;
  public float targetLineMaxWidth = 0.05f;

  private LineRenderer _lineRenderer;

	// Use this for initialization
	void Start () {
    _lineRenderer = gameObject.AddComponent<LineRenderer>();
    _lineRenderer.startWidth = targetLineMaxWidth;
    _lineRenderer.endWidth = targetLineMaxWidth;
    _lineRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

    if (Input.GetKeyDown(KeyCode.E))
    {
      if (currentlyHeld)
        Drop();
      else
        Pickup();
    }

    if (currentlyHeld)
      currentlyHeld.transform.position = guide.position;

    if (Input.GetMouseButton(0))
    {
      if (currentlyHeld)
      {
        DrawThrowTarget();
        if (_currentThrowForce < maxThrowForce)
        {
          _currentThrowForce += throwChargeRate * Time.deltaTime; // Multiply by deltatime to ensure it's framerate-independant
          if (_currentThrowForce >= maxThrowForce)
            _currentThrowForce = maxThrowForce;
        }
      }
      else
      {
        _currentThrowForce = 0.0f;
        _lineRenderer.enabled = false;
      }
        
    }

    if (Input.GetMouseButtonUp(0))
    {
      Throw();
    }
		
	}

  void OnTriggerEnter(Collider col)
  {
    if (col.gameObject.tag == "torch")
    {
      if (!torch)
        torch = col.gameObject;
    }
    if (col.gameObject.tag == "lantern")
    {
      if (!lantern)
        lantern = col.gameObject;
    }
  }

  void OnTriggerStay(Collider col)
  {
    if (col.gameObject.tag == "torch")
    {
      if (!torch)
        torch = col.gameObject;
    }
    if (col.gameObject.tag == "lantern")
    {
      if (!lantern)
        lantern = col.gameObject;
    }
  }

  void OnTriggerExit(Collider col)
  {
    if (col.gameObject.tag == "torch")
    {
      if (torch)
        torch = null;
    }
    if (col.gameObject.tag == "lantern")
    {
      if (lantern)
        lantern = null;
    }
  }

  private void Pickup()
  {
    if (!(torch || lantern))
      return;
    if (torch)
    {
      torch.GetComponent<Rigidbody>().isKinematic = true;
      torch.transform.SetParent(guide);
      torch.transform.localRotation = guide.rotation;
      torch.transform.Rotate(0, 0, 180); // Rotate otherwise it's upside-down...
      torch.transform.position = guide.position;
      currentlyHeld = torch;
    }
    else if (lantern)
    {
      lantern.GetComponent<Rigidbody>().isKinematic = true;
      lantern.transform.SetParent(guide);
      lantern.transform.localRotation = guide.rotation;
      lantern.transform.Rotate(-90, 0, 0); // Rotate for 3D model
      lantern.transform.position = guide.position;
      currentlyHeld = lantern;
    }
  }

  private void Drop()
  {
    if (!currentlyHeld)
      return;
    if (currentlyHeld == torch)
    {
      torch = null;
    }
    if (currentlyHeld == lantern)
    {
      lantern = null;
    }
    currentlyHeld.GetComponent<Rigidbody>().isKinematic = false;
    guide.GetChild(0).parent = null;
    currentlyHeld = null;
    if (torch || lantern)
    {
      Pickup();
    }
  }

  private void DrawThrowTarget()
  {
    if (!currentlyHeld)
      return;

    Vector3 mousePos = Input.mousePosition;
    Ray castPoint = Camera.main.ScreenPointToRay(mousePos);
    RaycastHit hit;
    if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
    {
      Vector3 direction = (hit.point - currentlyHeld.transform.position).normalized;

      float forceRatio = (_currentThrowForce / maxThrowForce);

      Vector3 target = currentlyHeld.transform.position + forceRatio * direction * targetLineLengthModifier;
      Vector3[] positions = { currentlyHeld.transform.position, target };

      _lineRenderer.startWidth = targetLineMaxWidth * forceRatio;
      _lineRenderer.endWidth = targetLineMaxWidth * forceRatio;

      _lineRenderer.SetPositions(positions);
      _lineRenderer.enabled = true;
    }
    
  }

  private void Throw()
  {
    if (!currentlyHeld)
      return;

    Vector3 mousePos = Input.mousePosition;
    Ray castPoint = Camera.main.ScreenPointToRay(mousePos);
    RaycastHit hit;
    if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
    {
      if (currentlyHeld == torch)
        torch = null;
      if (currentlyHeld == lantern)
        lantern = null;
      currentlyHeld.GetComponent<Rigidbody>().isKinematic = false;
      guide.GetChild(0).parent = null;
      Vector3 toMouse = hit.point - currentlyHeld.transform.position;
      toMouse.x = 0; // Remove the useless depth component
      currentlyHeld.GetComponent<Rigidbody>().AddForce(toMouse.normalized * _currentThrowForce);
      currentlyHeld.GetComponent<Rigidbody>().AddTorque(Vector3.Cross(new Vector3(0,1,0), (toMouse.normalized * _currentThrowForce)));
      currentlyHeld = null;
    }
    _currentThrowForce = 0.0f;
    _lineRenderer.enabled = false;
  }
}
