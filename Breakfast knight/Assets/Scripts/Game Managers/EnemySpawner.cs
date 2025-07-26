using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Agrega esto al inicio del archivo

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemigos; // Array de prefabs de enemigos
    public int poolSize = 10; // Tamaño del pool de enemigos
    public int maxActiveEnemies = 5; // Número máximo de enemigos activos al mismo tiempo
    public float spawnCooldown = 5f; // Tiempo de cooldown entre spawns en segundos
    public LayerMask playerLayer; // Capa del jugador
    public Transform pointA; // Primer punto del área de spawn
    public Transform pointB; // Segundo punto del área de spawn
    public Transform pointC; // Tercer punto del área de spawn
    public Transform pointD; // Cuarto punto del área de spawn
    public int maxEnemiesToKill = 20; // Cantidad máxima de enemigos a matar antes de desactivar el spawner
    private int killedEnemies = 0;    // Contador de enemigos eliminados
    private float lastSpawnTime;
    private int spawnCount = 0;
    private Queue<GameObject> enemyPool; // Pool de enemigos
    private List<GameObject> activeEnemies; // Lista de enemigos activos
    private bool isPlayerInRange = false; // Indica si el jugador está en el rango de detección
    private BoxCollider detectionBox; // BoxCollider para la detección
    public int[] enemiesPerWave = { 3, 5 }; // Cantidad de enemigos por ciclo de spawn
    private int currentWaveIndex = 0;
    public bool usarOleadas = false; // Habilita el sistema de oleadas
    public float delayEntreOleadas = 2f; // Delay entre oleadas en segundos
    private bool esperandoOleada = false;
    public float radioExclusionJugador = 3f; // Radio alrededor del jugador donde no se puede spawnear
    public Transform jugador; // Asigna el transform del jugador desde el inspector

    private void Start()
    {
        lastSpawnTime = -spawnCooldown; // Permitir spawnear inmediatamente al inicio
        CrearPool();
        activeEnemies = new List<GameObject>();

        // Obtener o agregar el BoxCollider
        detectionBox = GetComponent<BoxCollider>();
        if (detectionBox == null)
        {
            detectionBox = gameObject.AddComponent<BoxCollider>();
        }
        detectionBox.isTrigger = true;

    }

    private void Update()
    {
        if (!isPlayerInRange) return;

        if (usarOleadas)
        {
            // Solo spawnea la siguiente oleada si no hay enemigos activos y ha pasado el cooldown
            if (!esperandoOleada && activeEnemies.Count == 0 && Time.time >= lastSpawnTime + spawnCooldown)
            {
                StartCoroutine(SpawnOleadaConDelay());
            }
        }
        else
        {
            // Comportamiento original: spawnea si hay espacio y ha pasado el cooldown
            if (Time.time >= lastSpawnTime + spawnCooldown && activeEnemies.Count < maxActiveEnemies)
            {
                int cantidadASpawnear = enemiesPerWave[currentWaveIndex];
                int disponibles = maxActiveEnemies - activeEnemies.Count;
                int cantidadFinal = Mathf.Min(cantidadASpawnear, disponibles);

                for (int i = 0; i < cantidadFinal; i++)
                {
                    SpawnEnemigo();
                }

                lastSpawnTime = Time.time;
                currentWaveIndex = (currentWaveIndex + 1) % enemiesPerWave.Length;
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
            Vector3 spawnPosition;
            int intentos = 0;
            const int maxIntentos = 20;

            // Intenta encontrar una posición válida fuera del radio de exclusión
            do
            {
                spawnPosition = GetRandomPositionInArea();
                intentos++;
            }
            while (jugador != null && Vector3.Distance(spawnPosition, jugador.position) < radioExclusionJugador && intentos < maxIntentos);

            if (intentos >= maxIntentos)
            {
                Debug.LogWarning("No se encontró una posición válida para spawnear enemigo fuera del radio de exclusión.");
                return;
            }

            GameObject enemy = enemyPool.Dequeue();
            enemy.transform.position = spawnPosition;
            enemy.transform.rotation = Quaternion.identity;
            enemy.SetActive(true);
            activeEnemies.Add(enemy);
            Debug.Log("Enemigo spawneado y añadido a activeEnemies. Total de enemigos activos: " + activeEnemies.Count);
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

        for (int i = 0; i < 20; i++) // Intenta hasta 20 veces encontrar una posición válida
        {
            float randomX = UnityEngine.Random.Range(minX, maxX);
            float randomZ = UnityEngine.Random.Range(minZ, maxZ);
            Vector3 randomPosition = new Vector3(randomX, transform.position.y, randomZ);

            NavMeshHit hit;
            // Busca una posición válida en el NavMesh cerca de randomPosition, con un radio de 2 unidades
            if (NavMesh.SamplePosition(randomPosition, out hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        // Si no se encuentra una posición válida, retorna el centro del área como fallback
        float centerX = (minX + maxX) / 2f;
        float centerZ = (minZ + maxZ) / 2f;
        return new Vector3(centerX, transform.position.y, centerZ);
    }

    public void RegresarEnemigo(GameObject enemy)
    {
        // Llama al método Resetear si existe
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
            Debug.Log("Se alcanzó el límite de enemigos eliminados. Spawner desactivado.");
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
            if (jugador == null)
            {
                jugador = other.transform;
            }
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
        // Dibujar un cubo en el punto donde está el spawner con el tamaño del BoxCollider
        if (detectionBox != null)
        {
            Gizmos.DrawWireCube(transform.position + detectionBox.center, detectionBox.size);
        }

        // Dibujar el área de spawn
        if (pointA != null && pointB != null && pointC != null && pointD != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawLine(pointB.position, pointC.position);
            Gizmos.DrawLine(pointC.position, pointD.position);
            Gizmos.DrawLine(pointD.position, pointA.position);
        }
    }
    private IEnumerator SpawnOleadaConDelay()
    {
        esperandoOleada = true;
        yield return new WaitForSeconds(delayEntreOleadas);

        int cantidadASpawnear = enemiesPerWave[currentWaveIndex];
        int disponibles = Mathf.Min(cantidadASpawnear, maxActiveEnemies);

        for (int i = 0; i < disponibles; i++)
        {
            SpawnEnemigo();
        }

        lastSpawnTime = Time.time;
        currentWaveIndex = (currentWaveIndex + 1) % enemiesPerWave.Length;
        esperandoOleada = false;
    }
}
