using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureButton : MonoBehaviour {

    [SerializeField]
    LayerMask ActivatedBy;
    [SerializeField]
    PlatformMover[] target;

    bool status = false;
    bool Status
    {
        get { return status; }
        set
        {
            status = value;
            foreach (PlatformMover t in target)
            {
                if (t)
                    t.InteractedUpdate(status);
            }
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
