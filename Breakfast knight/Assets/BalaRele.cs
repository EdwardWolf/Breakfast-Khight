using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaRele : Bala
{
    [SerializeField] private float duracionRalentizacion = 2f; // Duraci�n del efecto de ralentizaci�n
    [SerializeField] private float factorRalentizacion = 0.5f; // Factor de ralentizaci�n

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag("Player"))
        {
            Jugador jugador = other.GetComponent<Jugador>();
            // Aplicar el efecto de aturdimiento
            jugador.AplicarRalentizacion(factorRalentizacion, duracionRalentizacion);

            // Reproducir sonido de impacto
            audioSource.volume = volumenImpacto;
            audioSource.PlayOneShot(impactoClip);

            // Regresar la bala despu�s del impacto
            attackHandler.ataqueActual.RegresarBala(this.gameObject);
        }
        else if (other.CompareTag("Muro"))
        {
            Debug.Log("Pego Muro");

            // Reproducir sonido de impacto
            audioSource.volume = volumenImpacto;
            audioSource.PlayOneShot(impactoClip);

            // Regresar la bala despu�s del impacto
            attackHandler.ataqueActual.RegresarBala(this.gameObject);
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

            // Reproducir sonido de impacto
            audioSource.volume = volumenImpacto;
            audioSource.PlayOneShot(impactoClip);

            // Regresar la bala despu�s del impacto
            attackHandler.ataqueActual.RegresarBala(this.gameObject);

        }
    }
}
