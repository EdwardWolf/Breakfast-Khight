using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AtaqueEnemigo : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int poolSize = 10;
    public float fireRate = 1f;
    private List<GameObject> bulletPool;
    private float nextFireTime;

    void Start()
    {
        CrearPool();
    }

    void CrearPool()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }

    public void Disparar(Vector3 direction)
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            GameObject bullet = ObtenerBala();
            if (bullet != null)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.LookRotation(direction);
                bullet.SetActive(true);
            }
        }
    }

    GameObject ObtenerBala()
    {
        foreach (var bala in bulletPool)
        {
            if (!bala.activeInHierarchy)
            {
                return bala;
            }
        }
        return null;
    }
}
