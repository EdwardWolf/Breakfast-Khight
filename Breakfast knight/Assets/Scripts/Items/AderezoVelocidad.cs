using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AderezoVelocidad : Aderezo
{
    public float incrementoVelocidad = 2f; // Incremento de velocidad de movimiento
    public float duracionEfecto = 5f; // Duración del efecto en segundos

    protected override void IncrementarVelocidadJugador()
    {
        if (jugador != null)
        {
            StartCoroutine(AplicarIncrementoVelocidadJugador());
            gameObject.SetActive(false); // Desactivar el objeto instanciado
            Debug.Log("Velocidad del jugador incrementada temporalmente");
        }
    }

    private IEnumerator AplicarIncrementoVelocidadJugador()
    {
        if (jugador != null)
        {
            jugador.velocidadActual += incrementoVelocidad;

            // Esperar la duración del efecto
            yield return new WaitForSeconds(duracionEfecto);

            // Verificar si el jugador sigue existiendo antes de restar la velocidad
            if (jugador != null)
            {
                jugador.velocidadActual = jugador._velocidadMovimiento;
            }
        }
        else
        {
            Debug.LogWarning("Referencia al jugador es nula. No se puede aplicar el incremento de velocidad.");
        }
    }


    protected override void IncrementarVelocidadEnemigo()
    {
        if (enemigo != null)
        {
            StartCoroutine(AplicarIncrementoVelocidadEnemigo());
            gameObject.SetActive(false); // Desactivar el objeto instanciado
            Debug.Log("Velocidad del enemigo incrementada temporalmente");
        }
    }

    private IEnumerator AplicarIncrementoVelocidadEnemigo()
    {
        if (enemigo != null)
        {
            enemigo.velocidadMovimientoInicial += incrementoVelocidad;
            yield return new WaitForSeconds(duracionEfecto);
            enemigo.velocidadMovimientoInicial -= incrementoVelocidad;
        }
    }
}
