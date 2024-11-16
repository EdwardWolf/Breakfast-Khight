using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AtaqueEnemigoPool : MonoBehaviour
{
    [SerializeField] public GameObject bulletPrefab; // Prefab de la bala.
    [SerializeField] public int bulletPool; // Cuantas balas se van a instanciar.
    [SerializeField] public int maxBulletInScene; // Cuantas balas se pueden tener en la escena activas simultaneamente.
    [SerializeField] public float fireRate; // Cada cuanto tiempo se puede disparar.
    [SerializeField] public float nextFireTime;
    private bool isAttacking = false;

    public Queue<GameObject> bulletPoolQueue = new Queue<GameObject>(); // Cola de balas.

    private void Start()
    {
        for (int i = 0; i < bulletPool; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.SetActive(false);
            bulletPoolQueue.Enqueue(bullet);
        }

        
    }


    private IEnumerator BulletPool()
    {
        while (true)
        {
            GameObject bullet = bulletPoolQueue.Dequeue();
            bullet.SetActive(true);
            StartCoroutine(DevolverALaCola(bullet));
            yield return new WaitForSeconds(fireRate);
        }
    }

    public IEnumerator DevolverALaCola(GameObject bullet)
    {
        yield return new WaitForSeconds(fireRate + 5f);
        bullet.SetActive(false);
        bulletPoolQueue.Enqueue(bullet);
    }

    public void Disparar()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            GameObject bullet = bulletPoolQueue.Dequeue();
            if (bullet != null)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
            }
        }
    }

    protected IEnumerator AttackCoroutine()
    {
        while (isAttacking)
        {
            Disparar();  // Llama al método de ataque abstracto (específico en clases derivadas)
            yield return new WaitForSeconds(1f / fireRate);  // Controla la cadencia de disparo
        }
    }

    // Comienza el ataque si no está ya atacando
    public void StartAttacking()
    {
        if (!isAttacking)
        {
            StartCoroutine(BulletPool());
            isAttacking = true;
            StartCoroutine(AttackCoroutine());
        }
    }

    // Detiene el ataque
    public void StopAttacking()
    {
        if (isAttacking)
        {
            isAttacking = false;
            StopCoroutine(AttackCoroutine());
        }
    }
}
