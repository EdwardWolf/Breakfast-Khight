using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image barraVelocidad;
    public Image barraAtaque;
    public Image iconoAderezo; // Imagen que representa el ataque
    public Sprite ItemVacio; // Imagen de vacío para el ataque
    public Image[] corazones; // Array de imágenes de corazones
    public Sprite corazonLleno;
    public Sprite corazonVacio;
    private const float valorCorazon = 10f; // Valor de cada corazón
    public Sprite spriteAderezoVelocidad;
    public Sprite spriteAderezoAtaque;
    private void OnEnable()
    {
        Jugador.OnVidaCambiada += ActualizarCorazones;
    }

    private void OnDisable()
    {
        Jugador.OnVidaCambiada -= ActualizarCorazones;
    }

    private void ActualizarCorazones(int corazonesActuales)
    {
        for (int i = 0; i < corazones.Length; i++)
        {
            if (i < corazonesActuales)
            {
                corazones[i].sprite = corazonLleno;
            }
            else
            {
                corazones[i].sprite = corazonVacio;
            }
        }
    }

    public void MostrarIncrementoAtaque(float incremento, float duracion)
    {
        if (barraAtaque != null)
            barraAtaque.gameObject.SetActive(true);

        if (iconoAderezo != null && spriteAderezoAtaque != null)
        {
            iconoAderezo.sprite = spriteAderezoAtaque;
            iconoAderezo.enabled = true;
        }
    }

    public void OcultarIncrementoAtaque()
    {
        if (barraAtaque != null)
            barraAtaque.gameObject.SetActive(false);

        if (iconoAderezo != null && ItemVacio != null)
            iconoAderezo.sprite = ItemVacio;
    }

    public void ActivarTemporalmente(GameObject objeto, float duracion)
    {
        StartCoroutine(ActivarObjetoTemporalmente(objeto, duracion));
    }

    private IEnumerator ActivarObjetoTemporalmente(GameObject objeto, float duracion)
    {
        objeto.SetActive(true);
        yield return new WaitForSeconds(duracion);
        objeto.SetActive(false);
    }

    public void MostrarIncrementoVelocidad(float incremento, float duracion)
    {
        barraVelocidad.gameObject.SetActive(true);
        if (barraVelocidad != null && spriteAderezoVelocidad != null)
        {
            iconoAderezo.sprite = spriteAderezoVelocidad;
        }
    }

    public void OcultarIncrementoVelocidad()
    {
        barraVelocidad.gameObject.SetActive(false);
        iconoAderezo.sprite = ItemVacio; // Cambiar a la imagen de vacío
    }
}
