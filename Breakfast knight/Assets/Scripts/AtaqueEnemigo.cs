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
    private Transform poolParent; // Nuevo padre para el pool de balas

    void Start()
    {
        CrearPool();
    }

    void CrearPool()
    {
        // Crear un nuevo objeto vacío para actuar como el padre del pool de balas
        poolParent = new GameObject("BulletPool").transform;

        bulletPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
            bullet.transform.SetParent(poolParent);
        }
    }

    public void RegresarBala(GameObject bala)
    {
        Debug.Log("Regresando bala al pool");
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

    protected GameObject ObtenerBalaConHandler(AttackHandler handler)
    {
        GameObject bala = ObtenerBala();
        if (bala != null)
        {
            Bala balaScript = bala.GetComponent<Bala>();
            if (balaScript != null)
            {
                balaScript.SetAttackHandler(handler);
            }
        }
        return bala;
    }

    public abstract void Atacar();

    public abstract void CoolDown();
}



