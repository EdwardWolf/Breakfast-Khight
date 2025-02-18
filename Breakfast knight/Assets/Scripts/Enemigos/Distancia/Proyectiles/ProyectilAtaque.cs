using UnityEngine;

public class ProyectilAtaque : AtaqueEnemigo
{
    public Transform firePoint; // Punto desde donde se disparará el proyectil

    public override void Atacar()
    {
        if (canShoot)
        {
            GameObject bullet = ObtenerBala();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.GetComponent<Rigidbody>().velocity = firePoint.forward * fireRate;
            canShoot = false;
            Invoke(nameof(CoolDown), fireRate);
        }
    }

    public override void CoolDown()
    {
        canShoot = true;
    }
}
