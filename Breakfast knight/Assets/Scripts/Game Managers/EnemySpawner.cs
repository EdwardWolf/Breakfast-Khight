using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemigos; // Array de prefabs de enemigos
    public int poolSize = 10; // Tama�o del pool de enemigos
    public int maxActiveEnemies = 5; // N�mero m�ximo de enemigos activos al mismo tiempo
    public float spawnCooldown = 5f; // Tiempo de cooldown entre spawns en segundos
    public LayerMask playerLayer; // Capa del jugador
    public Transform pointA; // Primer punto del �rea de spawn
    public Transform pointB; // Segundo punto del �rea de spawn
    public Transform pointC; // Tercer punto del �rea de spawn
    public Transform pointD; // Cuarto punto del �rea de spawn
    public int maxEnemiesToKill = 20; // Cantidad m�xima de enemigos a matar antes de desactivar el spawner
    private int killedEnemies = 0;    // Contador de enemigos eliminados
    private float lastSpawnTime;
    private int spawnCount = 0;
    private Queue<GameObject> enemyPool; // Pool de enemigos
    private List<GameObject> activeEnemies; // Lista de enemigos activos
    private bool isPlayerInRange = false; // Indica si el jugador est� en el rango de detecci�n
    private BoxCollider detectionBox; // BoxCollider para la detecci�n

    private void Start()
    {
        lastSpawnTime = -spawnCooldown; // Permitir spawnear inmediatamente al inicio
        CrearPool();
        activeEnemies = new List<GameObject>();
        detectionBox = gameObject.AddComponent<BoxCollider>();
        detectionBox.isTrigger = true;
        Debug.Log("Lista activeEnemies inicializada.");
    }

    private void Update()
    {
        if (isPlayerInRange && Time.time >= lastSpawnTime + spawnCooldown && activeEnemies.Count < maxActiveEnemies)
        {
            SpawnEnemigo();
            lastSpawnTime = Time.time;
            spawnCount++;

            if (spawnCount >= 3)
            {
                spawnCount = 0;
            }
        }
    }

    private void CrearPool()
    {
        enemyPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, enemigos.Length);
            GameObject enemy = Instantiate(enemigos[randomIndex]);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
            enemy.transform.SetParent(this.transform);
        }
    }

    private void SpawnEnemigo()
    {
        if (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            Vector3 spawnPosition = GetRandomPositionInArea();
            enemy.transform.position = spawnPosition;
            enemy.transform.rotation = Quaternion.identity;
            enemy.SetActive(true);
            activeEnemies.Add(enemy);
            Debug.Log("Enemigo spawneado y a�adido a activeEnemies. Total de enemigos activos: " + activeEnemies.Count);
        }
        else
        {
            Debug.LogWarning("No hay enemigos disponibles en el pool.");
        }
    }

    private Vector3 GetRandomPositionInArea()
    {
        float minX = Mathf.Min(pointA.position.x, pointB.position.x, pointC.position.x, pointD.position.x);
        float maxX = Mathf.Max(pointA.position.x, pointB.position.x, pointC.position.x, pointD.position.x);
        float minZ = Mathf.Min(pointA.position.z, pointB.position.z, pointC.position.z, pointD.position.z);
        float maxZ = Mathf.Max(pointA.position.z, pointB.position.z, pointC.position.z, pointD.position.z);

        float randomX = UnityEngine.Random.Range(minX, maxX);
        float randomZ = UnityEngine.Random.Range(minZ, maxZ);

        return new Vector3(randomX, transform.position.y, randomZ);
    }

    public void RegresarEnemigo(GameObject enemy)
    {
        // Llama al m�todo Resetear si existe
        var enemigoScript = enemy.GetComponent<Enemigo>();
        if (enemigoScript != null)
        {
            enemigoScript.Resetear();
        }

        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
        activeEnemies.Remove(enemy);
        killedEnemies++; // Incrementa el contador de enemigos eliminados
        Debug.Log("Enemigo regresado al pool y eliminado de activeEnemies. Total de enemigos activos: " + activeEnemies.Count);

        if (killedEnemies >= maxEnemiesToKill)
        {
            Debug.Log("Se alcanz� el l�mite de enemigos eliminados. Spawner desactivado.");
            gameObject.SetActive(false);
        }
    }

    private void CheckAndDeactivateSpawner()
    {
        if (activeEnemies.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            isPlayerInRange = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        // Dibujar un cubo en el punto donde est� el spawner con el tama�o del BoxCollider
        if (detectionBox != null)
        {
            Gizmos.DrawWireCube(transform.position + detectionBox.center, detectionBox.size);
        }

        // Dibujar el �rea de spawn
        if (pointA != null && pointB != null && pointC != null && pointD != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawLine(pointB.position, pointC.position);
            Gizmos.DrawLine(pointC.position, pointD.position);
            Gizmos.DrawLine(pointD.position, pointA.position);
        }
    }
}
