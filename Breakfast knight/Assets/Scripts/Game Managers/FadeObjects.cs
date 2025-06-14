using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FadeObjects : MonoBehaviour
{
    public float fadeSpeed, fadeAmount;
    float originalOpacity;
    Material[] Mats;
    public bool DoFade = false;

    void Start()
    {
        Mats = GetComponentsInChildren<Renderer>()[0].materials;
        foreach(Material mat in Mats)
        {
            originalOpacity = Mats[0].color.a;
        }

    }

    void Update()
    {
        if (DoFade)
        {
            FadeNow();
        }
        else
        {
            ResetFade();
        }
    }

    void FadeNow()
    {
        foreach (Material mat in Mats)
        {
            Color currentColor = mat.color;
            Color smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b,
                Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));
            mat.color = smoothColor;
        }
    }

    void ResetFade()
    {
        foreach(Material mat in Mats)
        {
            Color currentColor = mat.color;
            Color smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b,
                Mathf.Lerp(currentColor.a,originalOpacity,fadeSpeed * Time.deltaTime));
            mat.color = smoothColor;
        }
    }
}
