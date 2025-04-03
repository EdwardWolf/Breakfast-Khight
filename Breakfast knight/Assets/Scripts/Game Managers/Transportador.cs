using UnityEngine;
using System.Collections;

public class Transportador : MonoBehaviour
{
    public Transform puntoDestino; // Punto al que se transportará el jugador
    public float cantidadDanio = 10f; // Cantidad de salud que se restará al jugador
    public float delay = 2f; // Tiempo de retraso antes de transportar al jugador

    private void OnTriggerEnter(Collider other)
    {
        Jugador jugador = other.GetComponent<Jugador>();
        if (jugador != null)
        {
            StartCoroutine(TransportarConRetraso(jugador));
        }
    }

    private IEnumerator TransportarConRetraso(Jugador jugador)
    {
        // Restar salud al jugador
        jugador.ReducirVida(cantidadDanio);

        // Desactivar el movimiento del jugador
        jugador._velocidadMovimiento = 0f;

        // Esperar el tiempo de retraso
        yield return new WaitForSeconds(delay);

        // Transportar al jugador al punto de destino
        jugador.transform.position = puntoDestino.position;

        // Reactivar el movimiento del jugador
        jugador._velocidadMovimiento = jugador.stats.velocidadMovimiento;
    }
}
