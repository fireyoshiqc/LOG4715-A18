using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchActions : MonoBehaviour {

  public GameObject currentHeldItem;
  public GameObject otherItem;
  public bool canHold = true;
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
      if (!canHold)
        Drop();
      else
        Pickup();
    }

    if (!canHold && currentHeldItem)
      currentHeldItem.transform.position = guide.position;

    if (Input.GetMouseButton(0))
    {
      if (!canHold && currentHeldItem)
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
    if (col.gameObject.tag == "torch" || col.gameObject.tag == "lantern")
    {
      if (!currentHeldItem)
        currentHeldItem = col.gameObject;
      else
        otherItem = col.gameObject;
    }
      

  }

  void OnTriggerExit(Collider col)
  {
    if (col.gameObject.tag == "torch" || col.gameObject.tag == "lantern")
      {
      if (canHold)
        currentHeldItem = null;
      else
        otherItem = null;
    }
  }

  private void Pickup()
  {
    if (!currentHeldItem)
      return;

    currentHeldItem.GetComponent<Rigidbody>().isKinematic = true;
    currentHeldItem.transform.SetParent(guide);
    currentHeldItem.transform.localRotation = guide.rotation;
    if (currentHeldItem.tag == "torch")
      currentHeldItem.transform.Rotate(0, 0, 180); // Rotate otherwise it's upside-down...
    if (currentHeldItem.tag == "lantern")
      currentHeldItem.transform.Rotate(-90, 0, 0); // Rotate otherwise it's sideways

    currentHeldItem.transform.position = guide.position;

    canHold = false;
  }

  private void Drop()
  {
    if (!currentHeldItem)
    {
      return;
    }
    currentHeldItem.GetComponent<Rigidbody>().isKinematic = false;
    guide.GetChild(0).parent = null;
    canHold = true;
    if (otherItem)
    {
      currentHeldItem = otherItem;
      otherItem = null;
      Pickup();
    }
  }

  private void DrawThrowTarget()
  {
    if (!currentHeldItem)
      return;

    Vector3 mousePos = Input.mousePosition;
    Ray castPoint = Camera.main.ScreenPointToRay(mousePos);
    RaycastHit hit;
    if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
    {
      Vector3 direction = (hit.point - currentHeldItem.transform.position).normalized;

      float forceRatio = (_currentThrowForce / maxThrowForce);

      Vector3 target = currentHeldItem.transform.position + forceRatio * direction * targetLineLengthModifier;
      Vector3[] positions = { currentHeldItem.transform.position, target };

      _lineRenderer.startWidth = targetLineMaxWidth * forceRatio;
      _lineRenderer.endWidth = targetLineMaxWidth * forceRatio;

      _lineRenderer.SetPositions(positions);
      _lineRenderer.enabled = true;
    }
    
  }

  private void Throw()
  {
    if (!currentHeldItem)
      return;

    Vector3 mousePos = Input.mousePosition;
    Ray castPoint = Camera.main.ScreenPointToRay(mousePos);
    RaycastHit hit;
    if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
    {
      currentHeldItem.GetComponent<Rigidbody>().isKinematic = false;
      guide.GetChild(0).parent = null;
      Vector3 toMouse = hit.point - currentHeldItem.transform.position;
      toMouse.x = 0; // Remove the useless depth component
      currentHeldItem.GetComponent<Rigidbody>().AddForce(toMouse.normalized * _currentThrowForce);
      currentHeldItem.GetComponent<Rigidbody>().AddTorque(Vector3.Cross(new Vector3(0,1,0), (toMouse.normalized * _currentThrowForce)));
      canHold = true;
    }
    _currentThrowForce = 0.0f;
    _lineRenderer.enabled = false;
  }
}
