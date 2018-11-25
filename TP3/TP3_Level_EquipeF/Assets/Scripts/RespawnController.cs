using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour {

    public GameObject CurrentTorch; //Torche qui a trigger le checkpoint, last time we checked
    private Vector3 SavedPos;
    private Quaternion SavedRot;
    public GameObject[] ResetList;
    public TorchActions TA;

    private Dictionary<GameObject, Vector3> OriginalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> OriginalRotations = new Dictionary<GameObject, Quaternion>();

    // Use this for initialization
    void Awake ()
    {
        TriggerCheckpoint(CurrentTorch);
        foreach (GameObject obj in ResetList)
        {
            OriginalPositions.Add(obj, obj.transform.position);
            OriginalRotations.Add(obj, obj.transform.rotation);
        }
    }
	
	public void TriggerCheckpoint(GameObject detectedTorch)
    {
        CurrentTorch = detectedTorch;
        SavedPos = detectedTorch.transform.position;
        SavedRot = detectedTorch.transform.rotation;
    }

    public void ResetPositions()
    {
        Vector3 v;
        Quaternion q;
        TA.Drop();

        foreach (GameObject obj in ResetList)
        {
            if (OriginalPositions.TryGetValue(obj, out v) && OriginalRotations.TryGetValue(obj, out q))
            {
                obj.transform.SetPositionAndRotation(v, q);
                obj.SendMessage("ResetObject", SendMessageOptions.DontRequireReceiver);
            }
        }

        CurrentTorch.transform.SetPositionAndRotation(SavedPos, SavedRot);
    }
}
