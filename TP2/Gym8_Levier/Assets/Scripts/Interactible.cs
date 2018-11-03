﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour {

    enum SwitchType { Permanent, Toggled, Timed };

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


    [SerializeField]
    SwitchType switchType;
    [SerializeField]
    float time;
    [SerializeField]
    GameObject[] target;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateInteraction()
    {
        switch (switchType)
        {
            case SwitchType.Permanent:
                if (!Status) { Status = true; }
                break;
            case SwitchType.Toggled:
                Status = !Status;
                break;
            case SwitchType.Timed:
                if (!status)
                {
                    Status = true;
                    StartCoroutine(Timer());
                }
                break;
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(time);
        Status = false;
        //Possibility of graphical feedback that this is timed
        //Updates 10 times per second; could notify animations and the likes
        //yield return new WaitForSeconds(.1f);
    }
}