using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHijo : MonoBehaviour
{
    private void Start()
    {
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
                enemigo.RecibirDanio(10f); // Ajusta la cantidad de daño según sea necesario
            }
        }
    }
}
