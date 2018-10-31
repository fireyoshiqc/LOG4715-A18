using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchActions : MonoBehaviour {

  public GameObject torch;
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

    if (!canHold && torch)
      torch.transform.position = guide.position;

    if (Input.GetMouseButton(0))
    {
      if (!canHold && torch)
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
      if (!torch)
        torch = col.gameObject;

  }

  void OnTriggerExit(Collider col)
  {
    if (col.gameObject.tag == "torch")
    {
      if (canHold)
        torch = null;
    }
  }

  private void Pickup()
  {
    if (!torch)
      return;

    torch.GetComponent<Rigidbody>().isKinematic = true;
    torch.transform.SetParent(guide);

    torch.transform.localRotation = guide.rotation;
    torch.transform.Rotate(0, 0, 180); // Rotate otherwise it's upside-down...

    torch.transform.position = guide.position;

    canHold = false;
  }

  private void Drop()
  {
    if (!torch)
    {
      return;
    }
    torch.GetComponent<Rigidbody>().isKinematic = false;
    guide.GetChild(0).parent = null;
    canHold = true;
  }

  private void DrawThrowTarget()
  {
    if (!torch)
      return;

    Vector3 mousePos = Input.mousePosition;
    Ray castPoint = Camera.main.ScreenPointToRay(mousePos);
    RaycastHit hit;
    if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
    {
      Vector3 direction = (hit.point - torch.transform.position).normalized;

      float forceRatio = (_currentThrowForce / maxThrowForce);

      Vector3 target = torch.transform.position + forceRatio * direction * targetLineLengthModifier;
      Vector3[] positions = { torch.transform.position, target };

      _lineRenderer.startWidth = targetLineMaxWidth * forceRatio;
      _lineRenderer.endWidth = targetLineMaxWidth * forceRatio;

      _lineRenderer.SetPositions(positions);
      _lineRenderer.enabled = true;
    }
    
  }

  private void Throw()
  {
    if (!torch)
      return;

    Vector3 mousePos = Input.mousePosition;
    Ray castPoint = Camera.main.ScreenPointToRay(mousePos);
    RaycastHit hit;
    if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
    {
      torch.GetComponent<Rigidbody>().isKinematic = false;
      guide.GetChild(0).parent = null;
      Vector3 toMouse = hit.point - torch.transform.position;
      toMouse.x = 0; // Remove the useless depth component
      torch.GetComponent<Rigidbody>().AddForce(toMouse.normalized * _currentThrowForce);
      torch.GetComponent<Rigidbody>().AddTorque(Vector3.Cross(new Vector3(0,1,0), (toMouse.normalized * _currentThrowForce)));
      canHold = true;
    }
    _currentThrowForce = 0.0f;
    _lineRenderer.enabled = false;
  }
}
