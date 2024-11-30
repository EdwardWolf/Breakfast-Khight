using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Bala : MonoBehaviour
{
    [SerializeField]private AttackHandler attackHandler;

    private void Start()
    {
        Transform enemigo = transform.parent?.parent?.parent;

        if (enemigo != null)
        {
            // Obtener el componente del tercer padre
            attackHandler = enemigo.GetComponent<AttackHandler>();

            if (attackHandler != null)
            {
                // Hacer algo con el componente
                Debug.Log("Componente encontrado en el tercer padre.");
            }
            else
            {
                Debug.Log("El tercer padre no tiene el componente requerido.");
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Golpeado");
            attackHandler.ataqueActual.RegresarBala(this.gameObject);

        }
        if (other.CompareTag("Muro"))
        {
            Debug.Log("Pego Muro");
            attackHandler.ataqueActual.RegresarBala(this.gameObject);

        }
    }
}
