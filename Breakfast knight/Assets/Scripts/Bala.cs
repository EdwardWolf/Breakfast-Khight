using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    public AttackHandler attackHandler;
    [SerializeField] private float damage = 10f; // Daño que la bala inflige al jugador
    [SerializeField] private float shieldDamage = 5f; // Daño que la bala inflige al escudo

    public void SetAttackHandler(AttackHandler handler)
    {
        attackHandler = handler;
    }

    private void Start()
    {
        // Obtener el AttackHandler del tercer nivel de padres
        //Transform parent = transform.parent?.parent?.parent;
        Transform parent = transform.parent;
        if (parent != null)
        {
            attackHandler = parent.GetComponent<AttackHandler>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Golpeado");

            // Reducir la vida del jugador
            Jugador jugador = other.GetComponent<Jugador>();
            if (jugador != null)
            {
                jugador.ReducirVida(damage);
            }

            if (attackHandler != null && attackHandler.ataqueActual != null)
            {
                attackHandler.ataqueActual.RegresarBala(this.gameObject);
            }
        }
        else if (other.CompareTag("Muro"))
        {
            Debug.Log("Pego Muro");
            if (attackHandler != null && attackHandler.ataqueActual != null)
            {
                attackHandler.ataqueActual.RegresarBala(this.gameObject);
            }
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

            if (attackHandler != null && attackHandler.ataqueActual != null)
            {
                attackHandler.ataqueActual.RegresarBala(this.gameObject);
            }
        }
    }
}
