using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AderezoVelocidad : Aderezo
{
    public float incrementoVelocidad = 2f; // Incremento de velocidad de movimiento
    public float duracionEfecto = 5f; // Duración del efecto en segundos

    protected override void IncrementarVelocidadJugador()
    {
        if (jugador != null)
        {
            jugador.AplicarBuffVelocidad(incrementoVelocidad, duracionEfecto);
            jugador.BarrVelocidad(); // Actualizar la UI de velocidad del jugador
            gameObject.SetActive(false);
            Debug.Log("Velocidad del jugador incrementada temporalmente");
        }
    }
    //private IEnumerator AplicarIncrementoVelocidadJugador()
    //{
    //    if (jugador != null)
    //    {
    //        jugador.velocidadActual += incrementoVelocidad;

    //        if (velocidadEffectBar != null)
    //        {
    //            velocidadEffectBar.gameObject.SetActive(true);
    //        }

    //        float tiempoTranscurrido = 0f;
    //        while (tiempoTranscurrido < duracionEfecto)
    //        {
    //            tiempoTranscurrido += Time.deltaTime;
    //            if (velocidadEffectBar != null)
    //            {
    //                Debug.Log("Velocidad del jugador Normal");
    //                velocidadEffectBar.fillAmount = 1f - (tiempoTranscurrido / duracionEfecto);
    //            }
    //            yield return null;
    //        }

    //        if (velocidadEffectBar != null)
    //        {
    //            velocidadEffectBar.fillAmount = 0f;
    //            velocidadEffectBar.gameObject.SetActive(false);
    //        }

    //        if (jugador != null)
    //        {
    //            jugador.velocidadActual = jugador._velocidadMovimiento;
    //        }

    //        // Ahora sí, desactiva el objeto
    //        gameObject.SetActive(false);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Referencia al jugador es nula. No se puede aplicar el incremento de velocidad.");
    //    }
    //}


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
