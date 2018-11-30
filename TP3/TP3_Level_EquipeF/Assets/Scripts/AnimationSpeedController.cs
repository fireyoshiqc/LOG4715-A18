using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeedController : MonoBehaviour {
    public float animationSpeedRatio = 1.0f;
    private Animation anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animation>();
        if (anim)
        {
            foreach (AnimationState state in anim)
            {
                state.speed *= animationSpeedRatio;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
