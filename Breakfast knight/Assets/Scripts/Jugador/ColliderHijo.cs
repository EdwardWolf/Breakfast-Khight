using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHijo : MonoBehaviour
{
    private Jugador jugador;
    [SerializeField] private TrailRenderer trailRenderer; // Ahora es [SerializeField] para asignarlo desde el inspector

    private void Start()
    {
        jugador = GetComponentInParent<Jugador>();

        //// Validar si el TrailRenderer est� asignado
        //if (trailRenderer != null)
        //{
        //    trailRenderer.enabled = false; // Desactivar el TrailRenderer al inicio
        //}

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
                // Aplicar da�o al enemigo
                enemigo.RecibirDanio(50f); // Usar el da�o actual del jugador
                //jugador.AsestarGolpe(); // Llamar a AsestarGolpe en el jugador
            }
        }
    }

    //// M�todo para activar el collider y el TrailRenderer
    //public void ActivarCollider()
    //{
    //    GetComponent<Collider>().enabled = true;

    //    if (trailRenderer != null)
    //    {
    //        trailRenderer.enabled = true;
    //    }
    //}

    //// M�todo para desactivar el collider y el TrailRenderer
    //public void DesactivarCollider()
    //{
    //    GetComponent<Collider>().enabled = false;

    //    if (trailRenderer != null)
    //    {
    //        trailRenderer.enabled = false;
    //    }
    //}
}
