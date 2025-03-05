using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoLanzador : Enemigo
{
    public GameObject proyectilPrefab; // Prefab del proyectil
    public Transform puntoLanzamiento; // Punto desde donde se lanzará el proyectil
    public float fuerzaLanzamiento = 10f; // Fuerza con la que se lanzará el proyectil
    public float tiempoEntreLanzamientos = 2f; // Tiempo entre lanzamientos
    private float tiempoUltimoLanzamiento; // Tiempo del último lanzamiento

    private Queue<GameObject> poolProyectiles; // Pool de proyectiles
    public int tamañoPool = 10; // Tamaño del pool
    private GameObject poolParent; // Objeto padre para el pool de proyectiles

    protected override void Start()
    {
        base.Start();
        poolProyectiles = new Queue<GameObject>();
        poolParent = new GameObject("PoolProyectiles"); // Crear el objeto padre para el pool

        for (int i = 0; i < tamañoPool; i++)
        {
            GameObject proyectil = Instantiate(proyectilPrefab, poolParent.transform);
            proyectil.SetActive(false);
            poolProyectiles.Enqueue(proyectil);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (playerTransform != null && IsPlayerInAttackRange() && Time.time >= tiempoUltimoLanzamiento + tiempoEntreLanzamientos)
        {
            LanzarProyectil();
            tiempoUltimoLanzamiento = Time.time;
        }
    }

    private void LanzarProyectil()
    {
        if (poolProyectiles.Count > 0)
        {
            GameObject proyectil = poolProyectiles.Dequeue();
            proyectil.transform.position = puntoLanzamiento.position;
            proyectil.transform.rotation = Quaternion.identity;
            proyectil.SetActive(true);

            Rigidbody rb = proyectil.GetComponent<Rigidbody>();
            Vector3 direccion = (playerTransform.position - puntoLanzamiento.position).normalized;
            rb.velocity = Vector3.zero; // Resetear la velocidad del proyectil
            rb.AddForce(direccion * fuerzaLanzamiento, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar la trayectoria del lanzamiento
        if (puntoLanzamiento != null && playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 direccion = (playerTransform.position - puntoLanzamiento.position).normalized;
            Vector3 puntoInicial = puntoLanzamiento.position;
            Vector3 puntoFinal = puntoInicial + direccion * fuerzaLanzamiento;
            Gizmos.DrawLine(puntoInicial, puntoFinal);
        }
    }
}
