using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlinePostProcess : MonoBehaviour
{
    public Material outlineMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, outlineMaterial);
    }
}