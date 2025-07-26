using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AERecto : AtaqueEnemigo
{
    public Transform jugador;
    public LayerMask playerLayer;
    public float projectileSpeed = 10f; // Nueva variable para la velocidad del proyectil

    public GameObject objetoAActivar; // Asigna este objeto desde el inspector

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
        // En lugar de activar/desactivar instantáneamente, usar una corrutina
        StartCoroutine(MostrarEfectoYDisparar());
    }

    private IEnumerator MostrarEfectoYDisparar()
    {
        // Activar el objeto
        if (objetoAActivar != null)
            objetoAActivar.SetActive(true);

        // Esperar un momento para que el efecto sea visible antes de disparar
        yield return new WaitForSeconds(0.5f); // Ajusta este tiempo según necesites

        // Obtener y disparar la bala
        GameObject bala = ObtenerBala();
        if (bala != null)
        {
            bala.transform.position = transform.position;
            bala.transform.rotation = transform.rotation;

            Vector3 direction = (jugador.position - transform.position).normalized;
            bala.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
        }

        // Esperar otro momento para que el efecto se muestre después del disparo
        yield return new WaitForSeconds(0.2f); // Ajusta este tiempo según necesites

        // Desactivar el objeto
        if (objetoAActivar != null)
            objetoAActivar.SetActive(false);
    }

    public override void CoolDown()
    {
        // Implementar lógica de cooldown si es necesario
    }
}
