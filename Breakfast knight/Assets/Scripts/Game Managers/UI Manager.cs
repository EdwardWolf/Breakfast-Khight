using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image ataqueImage;
    public TMP_Text golpesText;
    public int golpesDisponibles;
    public Image[] corazones; // Array de imágenes de corazones
    public Sprite corazonLleno;
    public Sprite corazonVacio;
    private const float valorCorazon = 10f; // Valor de cada corazón

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

    public void MostrarIncrementoAtaque(float incremento, int golpes)
    {
        ataqueImage.gameObject.SetActive(true);
        golpesDisponibles = golpes;

        ActualizarTextoGolpes();
        // lógica adicional para manejar diferentes tipos de armas

    }

    public void UsarGolpe()
    {
        if (golpesDisponibles > 0)
        {
            golpesDisponibles--;
            ActualizarTextoGolpes();
            if (golpesDisponibles == 0)
            {
                golpesText.text = "";
                //ataqueImage.gameObject.SetActive(false);
            }
        }
    }

    private void ActualizarTextoGolpes()
    {
        golpesText.text = "Golpes disponibles: " + golpesDisponibles;
    }

    //Cambia el color de la imagen de ataque temporalmente
    //public void CambiarColorAtaqueImage(Color color, float duracion)
    //{
    //    StartCoroutine(CambiarColorTemporalmente(color, duracion));
    //}


    //private IEnumerator CambiarColorTemporalmente(Color color, float duracion)
    //{
    //    Color colorOriginal = ataqueImage.color;
    //    ataqueImage.color = color;
    //    golpesText.text = "Sin potenciador";
    //    yield return new WaitForSeconds(duracion);
    //    ataqueImage.color = colorOriginal;
    //    golpesText.text = "";
    //}

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

    public void MostrarImagenAderezo(Sprite sprite)
    {
        ataqueImage.sprite = sprite;
        ataqueImage.gameObject.SetActive(true);
    }
}
