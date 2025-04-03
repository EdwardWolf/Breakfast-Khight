using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AderezoRelentizar : Aderezo
{
    public float reduccionVelocidad = 0.5f; // Factor de reducci�n de velocidad
    public float duracionDebufo = 5f; // Duraci�n del debufo en segundos

    private void OnCollisionEnter(Collision collision)
    {
        Enemigo enemigo = collision.gameObject.GetComponent<Enemigo>();
        if (enemigo != null)
        {
            enemigo.AplicarRalentizacion(reduccionVelocidad, duracionDebufo);
        }
    }
}
