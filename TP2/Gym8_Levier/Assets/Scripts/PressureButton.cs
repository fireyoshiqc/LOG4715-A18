using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureButton : MonoBehaviour {

    [SerializeField]
    LayerMask ActivatedBy;
    [SerializeField]
    GameObject[] target;

    bool status = false;
    bool Status
    {
        get { return status; }
        set
        {
            status = value;
            foreach (GameObject t in target)
            { t.SendMessage("InteractedUpdate", status, SendMessageOptions.DontRequireReceiver); }
        }
    }

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, ActivatedBy);
        //Something on the button!
        Status = (hitColliders.Length > 0);
    }
    
}
