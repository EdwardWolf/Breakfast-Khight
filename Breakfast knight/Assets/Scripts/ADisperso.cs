using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADisperso : AtaqueEnemigo
{
    public Transform[] firePoints; // Puntos desde donde se disparar�n las balas

    public override void Atacar()
    {
        Debug.Log("Ataque de dispersi�n");

        foreach (Transform firePoint in firePoints)
        {
            DispararBala(firePoint);
        }
    }

    private void DispararBala(Transform firePoint)
    {
        // Obtener una bala del pool
        GameObject bala = ObtenerBala();
        if (bala != null)
        {
            // Posicionar y rotar la bala en el firePoint
            bala.transform.position = firePoint.position;
            bala.transform.rotation = firePoint.rotation;

            // Aplicar la direcci�n en la que est� mirando el firePoint a la bala
            bala.GetComponent<Rigidbody>().velocity = firePoint.forward * fireRate;
        }
    }

    public override void CoolDown()
    {
        // Implementar l�gica de cooldown si es necesario
    }
}
