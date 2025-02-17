using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemigos; // Array de prefabs de enemigos
    public Transform[] spawnPoints; // Array de puntos de spawn
    public int poolSize = 10; // Tamaño del pool de enemigos
    public int maxActiveEnemies = 5; // Número máximo de enemigos activos al mismo tiempo
    public float spawnCooldown = 5f; // Tiempo de cooldown entre spawns en segundos
    public float detectionRadius = 10f; // Radio de detección del jugador
    public LayerMask playerLayer; // Capa del jugador
    private float lastSpawnTime;
    private int spawnCount = 0;
    private Queue<GameObject> enemyPool; // Pool de enemigos
    private List<GameObject> activeEnemies; // Lista de enemigos activos
    private bool isPlayerInRange = false; // Indica si el jugador está en el rango de detección

    private void Start()
    {
        lastSpawnTime = -spawnCooldown; // Permitir spawnear inmediatamente al inicio
        CrearPool();
        activeEnemies = new List<GameObject>();
    }

    private void Update()
    {
        DetectPlayer();

        if (isPlayerInRange && Time.time >= lastSpawnTime + spawnCooldown && activeEnemies.Count < maxActiveEnemies)
        {
            SpawnEnemigo();
            lastSpawnTime = Time.time;
            spawnCount++;

            if (spawnCount >= 3)
            {
                CambiarEnemigo();
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
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            enemy.transform.position = spawnPoints[spawnIndex].position;
            enemy.transform.rotation = spawnPoints[spawnIndex].rotation;
            enemy.SetActive(true);
            activeEnemies.Add(enemy);
        }
        else
        {
            Debug.LogWarning("No hay enemigos disponibles en el pool.");
        }
    }

    public void RegresarEnemigo(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
        activeEnemies.Remove(enemy);
    }

    private void CambiarEnemigo()
    {
        // Este método ya no es necesario si los enemigos son aleatorios en el pool
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        isPlayerInRange = hits.Length > 0;
    }

    private void OnDrawGizmosSelected()
    {
        // Cambiar el color de los Gizmos (opcional)
        Gizmos.color = Color.green;
        // Dibujar una esfera en el punto donde está el spawner con el radio de detección
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
