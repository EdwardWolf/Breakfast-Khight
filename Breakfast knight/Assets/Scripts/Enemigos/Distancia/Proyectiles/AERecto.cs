using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AERecto : AtaqueEnemigo
{
    public Transform jugador;
    public LayerMask playerLayer;
    public float projectileSpeed = 10f; // Nueva variable para la velocidad del proyectil

    private float tiempoUltimoDisparo = -Mathf.Infinity;

    private void Awake()
    {
        jugador = GameObject.FindGameObjectWithTag("Point").transform;
    }
    
    public override void Atacar()
    {
        // Solo dispara si ha pasado el tiempo suficiente desde el �ltimo disparo
        //if (Time.time >= tiempoUltimoDisparo + fireRate) // si se quiere en segundos
        if (Time.time >= tiempoUltimoDisparo + 1f / fireRate)
        {
            DispararBala();
            tiempoUltimoDisparo = Time.time;
        }
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

            // Aplicar la direcci�n a la bala usando projectileSpeed
            bala.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
        }
    }

    public override void CoolDown()
    {
        // Implementar l�gica de cooldown si es necesario
    }
}
