using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    public AttackHandler attackHandler;
    [SerializeField] private float damage = 10f; // Daño que la bala inflige al jugador

    public void SetAttackHandler(AttackHandler handler)
    {
        attackHandler = handler;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Golpeado");

            // Reducir la vida del jugador
            Jugador jugador = collision.collider.GetComponent<Jugador>();
            if (jugador != null)
            {
                jugador.ReducirVida(damage);
            }

            if (attackHandler != null && attackHandler.ataqueActual != null)
            {
                Debug.Log("Llamando a RegresarBala para el jugador");
                attackHandler.ataqueActual.RegresarBala(this.gameObject);
            }
        }
        else if (collision.collider.CompareTag("Muro"))
        {
            Debug.Log("Pego Muro");
            if (attackHandler != null && attackHandler.ataqueActual != null)
            {
                Debug.Log("Llamando a RegresarBala para el muro");
                attackHandler.ataqueActual.RegresarBala(this.gameObject);
            }
        }
    }
}



