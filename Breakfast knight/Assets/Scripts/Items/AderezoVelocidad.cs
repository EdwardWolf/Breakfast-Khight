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
            StartCoroutine(AplicarIncrementoVelocidad());
            gameObject.SetActive(false); // Desactivar el objeto instanciado
            Debug.Log("Velocidad del jugador incrementada temporalmente");
        }
    }

    private IEnumerator AplicarIncrementoVelocidad()
    {
        if (jugador != null)
        {
            jugador._velocidadMovimiento += incrementoVelocidad;
            yield return new WaitForSeconds(duracionEfecto);
            jugador._velocidadMovimiento -= incrementoVelocidad;
        }
        if(enemigo != null)
        {
            enemigo.velocidadMovimiento += incrementoVelocidad;
            yield return new WaitForSeconds(duracionEfecto);
            enemigo.velocidadMovimiento -= incrementoVelocidad;
        }
    }

    protected override void IncrementarVelocidadEnemigo()
    {
        if (enemigo != null)
        {
            StartCoroutine(AplicarIncrementoVelocidad());
            gameObject.SetActive(false); // Desactivar el objeto instanciado
            Debug.Log("Velocidad del enemigo incrementada temporalmente");
        }
    }

}
