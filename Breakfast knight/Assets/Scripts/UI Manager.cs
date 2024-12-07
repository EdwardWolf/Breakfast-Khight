using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image[] corazones; // Array de imágenes de corazones
    public Sprite corazonLleno;
    public Sprite corazonVacio;

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
}
