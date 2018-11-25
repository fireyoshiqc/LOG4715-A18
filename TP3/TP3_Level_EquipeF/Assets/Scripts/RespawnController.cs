using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour {

    public GameObject CurrentTorch;
    public GameObject[] ResetList;
    private Dictionary<GameObject, Vector3> OriginalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> OriginalRotations = new Dictionary<GameObject, Quaternion>();

    // Use this for initialization
    void Awake ()
    {
        
        foreach (GameObject obj in ResetList)
        {
            OriginalPositions.Add(obj, obj.transform.position);
            OriginalRotations.Add(obj, obj.transform.rotation);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ResetPositions()
    {
        Vector3 v;
        Quaternion q;
        foreach (GameObject obj in ResetList)
        {
            if (OriginalPositions.TryGetValue(obj, out v) && OriginalRotations.TryGetValue(obj, out q))
            {
                obj.transform.SetPositionAndRotation(v, q);
                obj.SendMessage("ResetObject", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
