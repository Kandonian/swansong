﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PostEffectControl : MonoBehaviour {
    
    public Material material;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, material);
    }
}
