using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AderezoSalud : Aderezo
{
    public float cantidadRecuperacion = 20f; // Cantidad de salud que recupera o aumenta

    protected override void ReuperarSaludJugador()
    {
        RecuperarSaludJugador();
    }

    protected override void ReuperarSaludEnemigo()
    {
        RecuperarVidaEnemigo();
    }

    private void RecuperarSaludJugador()
    {
        if (jugador != null)
        {
            float vidaMaxima = jugador.stats.vida; // Vida máxima del jugador
            float vidaActual = jugador.vidaActual;

            // Recuperar salud sin exceder la vida máxima
            jugador.vidaActual = Mathf.Min(vidaActual + cantidadRecuperacion, vidaMaxima);
            Debug.Log($"Salud del jugador después de usar el aderezo: {jugador.vidaActual}");
            jugador.ActualizarBarraDeVida();
            gameObject.SetActive(false); // Desactivar el objeto instanciado
        }
    }

    private void RecuperarVidaEnemigo()
    {
        if (enemigo != null)
        {
            float vidaMaxima = enemigo.statsEnemigo.vida; // Vida máxima del enemigo
            float vidaActual = enemigo.vidaE;

            // Recuperar salud sin exceder la vida máxima
            enemigo.vidaE = Mathf.Min(vidaActual + cantidadRecuperacion, vidaMaxima);
            Debug.Log($"Vida del enemigo después de usar el aderezo: {enemigo.vidaE}");
            enemigo.ActualizarBarraDeVida();
            gameObject.SetActive(false); // Desactivar el objeto instanciado
        }
    }

}
