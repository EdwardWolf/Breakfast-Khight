using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADisperso : AtaqueEnemigo
{
    public Transform[] firePoints; // Puntos desde donde se dispararán las balas

    public override void Atacar()
    {
        foreach (Transform firePoint in firePoints)
        {
            DispararBala(firePoint);
        }
    }

    // Conseguir el parent (metaball) 
    
    private void DispararBala(Transform firePoint)
    {
        // Obtener una bala del pool con el handler
        GameObject bala = ObtenerBalaConHandler(GetComponent<AttackHandler>()); // meatball.getcomponent<AttackHandler>
        if (bala != null)
        {
            // Posicionar y rotar la bala en el firePoint
            bala.transform.position = firePoint.position;
            bala.transform.rotation = firePoint.rotation;

            // Aplicar la dirección en la que está mirando el firePoint a la bala
            bala.GetComponent<Rigidbody>().velocity = firePoint.forward * fireRate;
        }
    }

    public override void CoolDown()
    {
        // Implementar lógica de cooldown si es necesario
    }
}



