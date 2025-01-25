using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHijo : MonoBehaviour
{
    private Jugador jugador;

    private void Start()
    {
        jugador = GetComponentInParent<Jugador>();
        // Desactivar el collider al inicio
        GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo"))
        {
            Enemigo enemigo = other.GetComponent<Enemigo>();
            if (enemigo != null)
            {
                Debug.Log("Golpeado enemigo");
                // Aplicar daño al enemigo
                enemigo.RecibirDanio(jugador.ataque); // Usar el daño actual del jugador
                //jugador.AsestarGolpe(); // Llamar a AsestarGolpe en el jugador
            }
        }
    }
}









