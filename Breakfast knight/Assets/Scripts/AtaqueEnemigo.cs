using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AtaqueEnemigo : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab de la bala.
    public int poolSize = 10; // Cuantas balas se van a instanciar.
    public float fireRate = 1f; // Cada cuanto
    private Queue<GameObject> bulletPool; // Lista de balas.
    // Quitamos la variable nextFireTime.
    public bool canShoot = true; // Saber si podemos disparar.

    void Start()
    {
        CrearPool();
    }

    void CrearPool()
    {
        bulletPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
            bullet.transform.SetParent(this.transform);
        }
    }

    public void RegresarBala(GameObject bala)
    {
        bala.SetActive(false);
        bulletPool.Enqueue(bala);
    }

    protected GameObject ObtenerBala()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bala = bulletPool.Dequeue();
            bala.SetActive(true);
            return bala;
        }
        else
        {
            Debug.LogWarning("No hay balas disponibles en el pool.");
            return null;
        }
    }

    public abstract void Atacar();

    public abstract void CoolDown();
}
