using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AERecto : AtaqueEnemigo
{
    public Transform jugador;
    public LayerMask playerLayer;

    public override void Atacar()
    {
        Debug.Log("Ataque de dispersión");
        DispararBala();
    }

    private void DispararBala()
    {
        // Obtener una bala del pool
        GameObject bala = ObtenerBala();
        if (bala != null)
        {
            // Posicionar y rotar la bala en la posición del enemigo
            bala.transform.position = transform.position;
            bala.transform.rotation = transform.rotation;

            // Calcular la dirección hacia el jugador
            Vector3 direction = (jugador.position - transform.position).normalized;

            // Aplicar la dirección a la bala
            bala.GetComponent<Rigidbody>().velocity = direction * fireRate;
        }
    }


    public override void CoolDown()
    {
        // Implementar lógica de cooldown si es necesario
    }
}
