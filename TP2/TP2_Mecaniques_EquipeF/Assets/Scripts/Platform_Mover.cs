using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Mover : MonoBehaviour {

    [SerializeField]
    Transform Origin;
    [SerializeField]
    Transform Target;
    [SerializeField]
    [Range(0.1f, 10)]
    float TimeToMove;

    float currentTime = 0;

    float timescale = -1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        currentTime = Mathf.Clamp(currentTime + Time.deltaTime * timescale, 0, TimeToMove);

        float move_ratio = (currentTime / TimeToMove);
        transform.localPosition = Vector3.Lerp(Origin.localPosition, Target.localPosition, move_ratio);
        transform.localRotation = Quaternion.Lerp(Origin.localRotation, Target.localRotation, move_ratio);
    }

    public void InteractedUpdate(bool status)
    {
        timescale = status ? 1f : -1f;
    }
}
