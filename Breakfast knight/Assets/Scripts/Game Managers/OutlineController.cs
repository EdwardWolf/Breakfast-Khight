using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Renderer characterRenderer;
    public Material outlineMaterial;

    void Update()
    {
        // Si el personaje está detrás de un objeto, aplica el outline
        if (!characterRenderer.isVisible)
        {
            ApplyOutline();
        }
        else
        {
            RemoveOutline();
        }
    }

    void ApplyOutline()
    {
        characterRenderer.material = outlineMaterial;
    }

    void RemoveOutline()
    {
        // Restaurar el material original
        // characterRenderer.material = originalMaterial;
    }
}
