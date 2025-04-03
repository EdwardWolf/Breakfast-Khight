using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AERecto : AtaqueEnemigo
{
    public Transform jugador;
    public LayerMask playerLayer;


    private void Awake()
    {
        jugador = GameObject.FindGameObjectWithTag("Point").transform;
    }
    
    public override void Atacar()
    {
        DispararBala();
    }

    private void DispararBala()
    {
        // Obtener una bala del pool
        GameObject bala = ObtenerBala();
        if (bala != null)
        {
            // Posicionar y rotar la bala en la posici�n del enemigo
            bala.transform.position = transform.position;
            bala.transform.rotation = transform.rotation;

            // Calcular la direcci�n hacia el jugador
            Vector3 direction = (jugador.position - transform.position).normalized;

            // Aplicar la direcci�n a la bala
            bala.GetComponent<Rigidbody>().velocity = direction * fireRate;
        }
    }


    public override void CoolDown()
    {
        // Implementar l�gica de cooldown si es necesario
    }
}
