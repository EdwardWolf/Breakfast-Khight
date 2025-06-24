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
        // Solo dispara si ha pasado el tiempo suficiente desde el último disparo
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
            // Posicionar y rotar la bala en la posición del enemigo
            bala.transform.position = transform.position;
            bala.transform.rotation = transform.rotation;

            // Calcular la dirección hacia el jugador
            Vector3 direction = (jugador.position - transform.position).normalized;

            // Aplicar la dirección a la bala usando projectileSpeed
            bala.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
        }
    }

    public override void CoolDown()
    {
        // Implementar lógica de cooldown si es necesario
    }
}
