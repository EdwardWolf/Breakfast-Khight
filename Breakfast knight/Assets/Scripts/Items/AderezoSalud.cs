using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AderezoSalud : Aderezo
{
    public float cantidadRecuperacion = 20f; // Cantidad de salud que recupera o aumenta

    protected override void IncrementarAtaqueJugador()
    {
        RecuperarSaludJugador();
    }

    protected override void IncrementarAtaqueEnemigo()
    {
        AumentarVidaEnemigo();
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
        }
    }

    private void AumentarVidaEnemigo()
    {
        if (enemigo != null)
        {
            enemigo.vidaE += cantidadRecuperacion; // Aumentar la vida del enemigo
            enemigo.ActualizarBarraDeVida(); // Actualizar la barra de vida del enemigo
            Debug.Log($"Vida del enemigo después de usar el aderezo: {enemigo.vidaE}");
        }
    }
}
