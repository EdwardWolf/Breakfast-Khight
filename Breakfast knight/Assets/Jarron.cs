using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jarron : MonoBehaviour
{
    // Configuraci�n de monedas
    [Header("Configuraci�n de monedas")]
    [Tooltip("Probabilidad (entre 0 y 1) de soltar monedas al romperse el jarr�n.")]
    [Range(0f, 1f)]
    public float dropProbability = 0.5f;

    [Tooltip("Cantidad de monedas a soltar cuando se cumple la probabilidad.")]
    public int coinsToDrop = 3;

    [Tooltip("Prefab de la moneda a instanciar al romper el jarr�n.")]
    [SerializeField] private GameObject coinPrefab;

    // Configuraci�n de dispersi�n
    [Header("Configuraci�n de dispersi�n")]
    [Tooltip("Radio en el cual se dispersan las monedas.")]
    public float spreadRadius = 0.5f;

    public void BreakVase()
    {
        Debug.Log("BreakVase llamado.");

        // Comparar la probabilidad para determinar si se deben soltar monedas
        if (Random.value <= dropProbability)
        {
            Debug.Log("Probabilidad cumplida, se instanciar�n monedas.");
            for (int i = 0; i < coinsToDrop; i++)
            {
                // Calcular una posici�n aleatoria cerca del jarr�n
                Vector3 randomOffset = new Vector3(Random.Range(-spreadRadius, spreadRadius), Random.Range(-spreadRadius, spreadRadius), 0);
                Vector3 spawnPosition = transform.position + randomOffset;

                // Instanciar el prefab de la moneda
                if (coinPrefab != null)
                {
                    Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
                    Debug.Log($"Moneda instanciada en: {spawnPosition}");
                }
                else
                {
                    Debug.LogWarning("coinPrefab no est� asignado.");
                }
            }
        }
        else
        {
            Debug.Log("Probabilidad no cumplida; no se instanciar�n monedas.");
        }

        // Aqu� se pueden agregar efectos o animaciones de ruptura
        Destroy(gameObject);
    }

    // Ejemplo de c�mo disparar la ruptura al detectar una colisi�n
    private void OnCollisionEnter(Collision collision)
    {
        // Se verifica si el objeto que colisiona pertenece al layer "arman"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Arma"))
        {
            Debug.Log("Jarr�n roto por colisi�n con objeto en el layer 'arman'.");
            BreakVase();
        }
    }
}
