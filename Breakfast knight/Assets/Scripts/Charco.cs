using System.Collections;
using UnityEngine;

public class Charco : MonoBehaviour
{
    public float tiempoDeEspera = 2f; // Tiempo de espera antes de comenzar a disminuir el albedo
    public float velocidadDeDisminucion = 0.5f; // Velocidad a la que disminuye el albedo

    private Renderer renderer;
    private Material material;
    private Color colorInicial;
    private Jugador jugador; // Referencia al jugador
    public float reduccionVelocidad = 0.3f; // Reducción de velocidad (30%)



    private void Start()
    {
        renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
            colorInicial = material.color;

        }
        else
        {
            Debug.LogError("No se encontró un Renderer en el objeto.");
        }
    }

    public void IniciarDisminucion()
    {

            StartCoroutine(DisminuirAlbedo());

    }

    private IEnumerator DisminuirAlbedo()
    {
        // Esperar el tiempo especificado antes de comenzar a disminuir el albedo
        yield return new WaitForSeconds(tiempoDeEspera);

        float albedo = colorInicial.a;

        while (albedo > 0)
        {
            albedo -= Time.deltaTime * velocidadDeDisminucion;
            albedo = Mathf.Clamp01(albedo); // Asegurarse de que el albedo no sea menor que 0

            Color nuevoColor = new Color(colorInicial.r, colorInicial.g, colorInicial.b, albedo);
            material.color = nuevoColor;

            yield return null;
        }

        // Desactivar el objeto una vez que el albedo sea 0
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugador = other.GetComponent<Jugador>();
            if (jugador != null)
            {
                jugador._velocidadMovimiento *= (1 - reduccionVelocidad); // Reducir la velocidad del jugador en un 30%
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && jugador != null)
        {
            jugador._velocidadMovimiento /= (1 - reduccionVelocidad); // Restaurar la velocidad del jugador
            jugador = null;
        }
    }
}
