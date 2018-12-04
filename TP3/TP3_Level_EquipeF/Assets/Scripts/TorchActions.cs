using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchActions : MonoBehaviour
{

    public GameObject torch;
    public GameObject lantern;
    public GameObject scepter;
    public GameObject currentlyHeld;
    public Transform guide;
    public Transform fireball;

    public float maxThrowForce = 500.0f;
    public float throwChargeRate = 350.0f;
    [Range(0.0f, 100.0f)]
    public float fireBallBaseSpeed = 3;
    //coût en % de la vie de la torche maximale pour un tir chargé au maximum
    [Range(0.0f, 1.0f)]
    public float scepterMaxFlameCost = 0.1f;
    [Range(0.0f, float.PositiveInfinity)]
    public float scepterShootDelay = 0.5f;
    private float currentScepterDelay = 0.0f;
    private float _currentThrowForce = 0.0f;

    public float targetLineLengthModifier = 3.0f;
    public float targetLineMaxWidth = 0.05f;

    private PlayerController Pc;

    private LineRenderer _lineRenderer;

    //le scale de la torche tenue
    private Vector3 scale;
    private GameObject emptyObject;

    // Use this for initialization
    void Start()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = targetLineMaxWidth;
        _lineRenderer.endWidth = targetLineMaxWidth;
        _lineRenderer.enabled = false;
        emptyObject = new GameObject();
        Pc = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentlyHeld)
                Drop();
            else
                Pickup();
        }*/

        if (currentlyHeld)
        {
            if (currentlyHeld != scepter)
                currentlyHeld.transform.position = guide.position;
            else
                scepter.transform.position = guide.position + new Vector3(0f, 1f, 0f);
        }

        if (!Pc.isCutsceneControlled)
        {
            if (Input.GetMouseButton(0))
            {
                if (currentlyHeld && (currentlyHeld != scepter || (currentlyHeld == scepter && currentScepterDelay < float.Epsilon)))
                {
                    if (_currentThrowForce < maxThrowForce)
                    {
                        _currentThrowForce += throwChargeRate * Time.deltaTime; // Multiply by deltatime to ensure it's framerate-independant
                        if (_currentThrowForce >= maxThrowForce)
                            _currentThrowForce = maxThrowForce;
                    }
                    DrawThrowTarget();
                }
                else
                {
                    _currentThrowForce = 0.0f;
                    _lineRenderer.enabled = false;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (currentlyHeld)
                {
                    if (currentlyHeld != scepter)
                    {
                        Throw();
                    }
                    else if (currentScepterDelay < float.Epsilon)
                    {
                        ShootFireBall();
                    }
                }
            }
        }
        currentScepterDelay = Mathf.Max(0.0f, currentScepterDelay - Time.deltaTime);
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
        if (col.gameObject.tag == "scepter")
        {
            if (!scepter)
                scepter = col.gameObject;
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
        if (col.gameObject.tag == "scepter")
        {
            if (!scepter)
                scepter = col.gameObject;
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
        if (col.gameObject.tag == "scepter")
        {
            if (scepter)
                scepter = null;
        }
    }

    public bool Pickup()
    {
        if (!(torch || lantern || scepter))
            return false; //Did nothing
        if (torch)
        {
            torch.GetComponent<Rigidbody>().isKinematic = true;
            scale = torch.transform.localScale;
            emptyObject.transform.parent = guide.transform;
            torch.transform.parent = emptyObject.transform;
            torch.transform.localScale = scale;
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
        else if (scepter)
        {
            scepter.GetComponent<Rigidbody>().isKinematic = true;
            scepter.transform.SetParent(guide);
            scepter.transform.localRotation = guide.rotation;
            scepter.transform.Rotate(-90, 0, 0); // Rotate for 3D model
            scepter.transform.position = guide.position + new Vector3(0f, 1f, 0f);
            currentlyHeld = scepter;
        }
        return true;
    }

    public void Drop()
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
        if (currentlyHeld == scepter)
        {
            scepter = null;
        }
        currentlyHeld.GetComponent<Rigidbody>().isKinematic = false;
        currentlyHeld.transform.parent = null;
        currentlyHeld = null;
        if (torch || lantern || scepter)
        {
            Pickup();
        }
    }

    private void DrawThrowTarget()
    {
        if (!currentlyHeld)
            return;



        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z + currentlyHeld.transform.position.z;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 direction = (worldPoint - currentlyHeld.transform.position);
        direction.z = 0;
        direction.Normalize();

        float forceRatio = (_currentThrowForce / maxThrowForce);

        Vector3 target = currentlyHeld.transform.position + forceRatio * direction * targetLineLengthModifier;
        //Vector3[] positions = { currentlyHeld.transform.position, target };

        _lineRenderer.startWidth = targetLineMaxWidth * forceRatio;
        _lineRenderer.endWidth = targetLineMaxWidth * forceRatio;
        List<Vector3> throwArc = CalculateThrowArc(currentlyHeld.transform.position, target);
        _lineRenderer.positionCount = throwArc.Count;
        _lineRenderer.SetPositions(throwArc.ToArray());
        _lineRenderer.enabled = true;
    }

    private List<Vector3> CalculateThrowArc(Vector3 source, Vector3 target)
    {
        List<Vector3> arcArray = new List<Vector3>();
        Vector3 lineOfAim = target - source;
        float angle = Mathf.Atan(lineOfAim.y / lineOfAim.x);
        if (lineOfAim.x < 0.0f)
        {
            angle += Mathf.PI;
        }
        if (lineOfAim.x > 0.0f)
        {
            angle += 2 * Mathf.PI;
        }
        float estimatedVelocity = lineOfAim.magnitude * (10.0f / targetLineLengthModifier);
        //float maxDistance = (estimatedVelocity * estimatedVelocity * Mathf.Sin(2 * angle)) / gravity;

        for (int i = 0; i < 50; i++)
        {
            float t = (float)i / 50.0f;
            float x = t * lineOfAim.x;
            float y = x * Mathf.Tan(angle) - ((Mathf.Abs(Physics.gravity.y) * x * x) / (2 * estimatedVelocity * estimatedVelocity * Mathf.Cos(angle) * Mathf.Cos(angle)));
            arcArray.Add(new Vector3(source.x + x, source.y + y, source.z));
            RaycastHit hitInfo;
            if (i > 0 && Physics.Linecast(arcArray[i - 1], arcArray[i], out hitInfo))
            {
                return arcArray;
            }
        }
        return arcArray;

    }

    private void Throw()
    {
        if (!currentlyHeld)
            return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z + currentlyHeld.transform.position.z;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        if (currentlyHeld == torch)
            torch = null;
        if (currentlyHeld == lantern)
            lantern = null;
        currentlyHeld.GetComponent<Rigidbody>().isKinematic = false;
        guide.GetChild(0).parent = null;
        Vector3 toMouse = worldPoint - currentlyHeld.transform.position;
        toMouse.z = 0; // Remove the useless depth component
        currentlyHeld.GetComponent<Rigidbody>().AddForce(toMouse.normalized * _currentThrowForce);
        currentlyHeld.GetComponent<Rigidbody>().AddTorque(Vector3.Cross(new Vector3(0, 1, 0), (toMouse.normalized * _currentThrowForce)));
        AudioSource sfx = currentlyHeld.GetComponent<AudioSource>();
        FlameController flame = currentlyHeld.GetComponent<FlameController>();
        if (sfx && flame && flame.flameLife > 0.0f)
        {
            sfx.Play();
        }
        currentlyHeld = null;

        _currentThrowForce = 0.0f;
        _lineRenderer.enabled = false;
    }

    private void ShootFireBall()
    {
        _lineRenderer.enabled = false;
        float ratio = Mathf.Clamp(_currentThrowForce / maxThrowForce, 0.2f, 1);
        _currentThrowForce = 0.0f;
        FlameController flame = scepter.GetComponent<FlameController>();
        if(flame.flameLife < float.Epsilon)
        {
            return;
        }
        float fireBallCost = flame.maxFlameLife * scepterMaxFlameCost * ratio;
        if (flame.flameLife < fireBallCost)
        {
            //trouver le ratio qui donne un coût égal à la vie restante
            ratio = flame.flameLife / (flame.maxFlameLife * scepterMaxFlameCost);
            //si le ratio est trop petit, aucune fireball n'apparaît
            if(ratio < 0.2f)
            {
                return;
            }
            fireBallCost = flame.flameLife;
        }
        flame.flameLife -= fireBallCost;

        Vector3 mousePos = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Rigidbody fireballClone = Instantiate(fireball, currentlyHeld.transform.position, currentlyHeld.transform.rotation).GetComponent<Rigidbody>();

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            Vector3 toMouse = hit.point - currentlyHeld.transform.position;
            toMouse.x = 0; // Remove the useless depth component
            fireballClone.velocity = toMouse.normalized * fireBallBaseSpeed * ratio;
        }
        currentScepterDelay = scepterShootDelay;
    }
}
