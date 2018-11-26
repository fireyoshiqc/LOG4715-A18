﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaAnimator : MonoBehaviour {

    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0.5f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offsetX = Time.time * scrollSpeedX;
        float offsetY = Time.time * scrollSpeedY;
        rend.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
        rend.material.SetTextureOffset("_BumpMap", new Vector2(offsetX, offsetY));
        rend.material.SetTextureOffset("_Illum", new Vector2(offsetX, offsetY));
    }
}
