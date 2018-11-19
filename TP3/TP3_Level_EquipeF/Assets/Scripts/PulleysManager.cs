using UnityEngine;
using System.Collections.Generic;

public class PulleysManager : MonoBehaviour {

    [SerializeField]
    GameObject[] linkedPlatforms;

    float totalMassOnSystem = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        totalMassOnSystem = 0;
		ComputeTotalMass();
        ApplyMassToPlatforms();
	}

    void ComputeTotalMass()
    {
        foreach (GameObject t in linkedPlatforms) {
            totalMassOnSystem += GetMassOnPlatform(t);
        }
    }

    void ApplyMassToPlatforms()
    {
        foreach (GameObject t in linkedPlatforms) {
            setMassOfSystemOnPlatform(t);
        }
    }

    float GetMassOnPlatform(GameObject t)
    {
        PulleyPlatform pulleyPlatform = t.GetComponent<PulleyPlatform>();
        float mass = pulleyPlatform.massOnPlatform;
        return (pulleyPlatform.isPositive) ? mass : -mass;
    }

    void setMassOfSystemOnPlatform(GameObject t)
    {
        PulleyPlatform pulleyPlatform = t.GetComponent<PulleyPlatform>();
        pulleyPlatform.massToApply = totalMassOnSystem;
    }
}
