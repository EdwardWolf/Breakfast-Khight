using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class RainbowEffect : MonoBehaviour
{
    public float speed = 1.0f; // Speed of the hue shift
    public float emissionIntensity = 1.0f; // Emission intensity
    [Range(0.0f, 1.0f)] public float emissionHueIntensity = 1.0f; // Emission hue intensity
    [Range(0.0f, 1.0f)] public float baseColorHueIntensity = 1.0f; // Base color hue intensity
    public float hueChangeInterval = 0.1f; // Interval between hue changes

    private Renderer objectRenderer;
    private MaterialPropertyBlock propertyBlock;
    private float hue = 0.0f;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        // Set initial white color for the emission
        SetMaterialColor(Color.white, Color.white);

        // Start the hue shifting coroutine
        StartCoroutine(ShiftHue());
    }

    private IEnumerator ShiftHue()
    {
        while (true)
        {
            // Increment hue based on speed
            hue += speed * hueChangeInterval;
            hue = hue % 1.0f; // Keep hue in the range [0, 1]

            // Convert hue to color for the base color
            Color baseColor = Color.HSVToRGB(hue, baseColorHueIntensity, 1.0f);

            // Convert hue to color with adjustable saturation for the emission color
            Color emissionColor = Color.HSVToRGB(hue, emissionHueIntensity, 1.0f);

            SetMaterialColor(baseColor, emissionColor);

            yield return new WaitForSeconds(hueChangeInterval);
        }
    }

    private void SetMaterialColor(Color baseColor, Color emissionColor)
    {
        // Set base color
        propertyBlock.SetColor("_BaseColor", baseColor);

        // Set emission color
        propertyBlock.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        propertyBlock.SetFloat("_EmissionIntensity", emissionIntensity);

        // Apply the property block to the renderer
        objectRenderer.SetPropertyBlock(propertyBlock);
    }
}