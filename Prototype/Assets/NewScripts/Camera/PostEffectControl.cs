using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PostEffectControl : MonoBehaviour {
    
    public Material material;
    public float vignette;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        material.SetFloat("_VignetteStrength", vignette);

        Graphics.Blit(src, dest, material);
    }
}
