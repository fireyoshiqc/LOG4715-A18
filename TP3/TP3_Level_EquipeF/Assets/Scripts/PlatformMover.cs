using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour {

    [SerializeField]
    Transform Origin;
    [SerializeField]
    Transform Target;
    [SerializeField]
    [Range(0.1f, 10)]
    float TimeToMove;

    float currentTime = 0;

    float timescale = -1;

    private List<GameObject> stuffOnIt;
    private List<Vector3> offsets;

	// Use this for initialization
	void Start () {
        stuffOnIt = new List<GameObject>();
        offsets = new List<Vector3>();
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

    void OnTriggerStay(Collider other)
    {
        stuffOnIt.Add(other.gameObject);
        offsets.Add(other.gameObject.transform.position - transform.position);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < stuffOnIt.Count; i++)
        {
            float oldZ = stuffOnIt[i].transform.position.z;
            stuffOnIt[i].transform.position = transform.position + offsets[i];
            stuffOnIt[i].transform.position = new Vector3(stuffOnIt[i].transform.position.x, stuffOnIt[i].transform.position.y, oldZ);
        }

        stuffOnIt.Clear();
        offsets.Clear();
    }
}
