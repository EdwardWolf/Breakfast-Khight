using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaStun : Bala
{
    [SerializeField] private float duracionAturdimiento = 2f; // Duración del efecto de aturdimiento

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reducir la vida del jugador
            Jugador jugador = other.GetComponent<Jugador>();
            if (jugador != null)
            {
                jugador.ReducirVida(damage);

                // Aplicar el efecto de aturdimiento
                jugador.AplicarAturdimiento(duracionAturdimiento);
            }

            // Reproducir el sonido del impacto
            if (audioSource != null && impactoClip != null)
            {
                audioSource.PlayOneShot(impactoClip);
            }

            // Regresar la bala después del impacto
            StartCoroutine(RegresarBalaDespuesDeTiempo(0f));
        }
        else if (other.CompareTag("Muro"))
        {
            Debug.Log("Pego Muro");
            // Regresar la bala después del impacto
            StartCoroutine(RegresarBalaDespuesDeTiempo(0f));
        }
        else if (other.CompareTag("Escudo"))
        {
            Debug.Log("Pego Escudo");

            // Reducir la resistencia del escudo
            Jugador jugador = other.GetComponentInParent<Jugador>();
            if (jugador != null)
            {
                jugador.ReducirResistenciaEscudo(shieldDamage);
            }

            // Regresar la bala después del impacto
            StartCoroutine(RegresarBalaDespuesDeTiempo(0f));
        }
    }
}
