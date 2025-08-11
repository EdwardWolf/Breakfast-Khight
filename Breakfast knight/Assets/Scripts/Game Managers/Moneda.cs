using System.Collections;
using UnityEngine;

public class Moneda : MonoBehaviour
{
    // Número de rebotes antes de la atracción.
    public int numberOfBounces = 2;
    // Fuerza aplicada en el rebote.
    public float bounceForce = 5f;
    // Velocidad a la que se atrae la moneda al jugador.
    public float attractSpeed = 10f;
    // Tiempo de espera entre rebotes.
    public float bounceDelay = 0.5f;
    // Distancia mínima para considerar que la moneda ha llegado al jugador.
    public float minDistanceForDestroy = 0.5f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }
        StartCoroutine(BounceAndAttract());
    }

    private IEnumerator BounceAndAttract()
    {
        // Ejecuta los rebotes
        for (int i = 0; i < numberOfBounces; i++)
        {
            // Reinicia la velocidad vertical para evitar acumulación.
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);
            yield return new WaitForSeconds(bounceDelay);
        }

        // Inicia la atracción al jugador
        Atraer();
        yield break;
    }

    private void Atraer()
    {
        // Se asume que existe un componente 'Jugador' en la escena
        Jugador player = FindObjectOfType<Jugador>();
        if (player == null)
        {
            FindObjectOfType<UIManager>().AgregarMonedas(1);
            Destroy(gameObject);
            return;
        }

        // Si existe un transform 'center' en el jugador, se utiliza; de lo contrario se usa el del jugador
        Transform target = player.center != null ? player.center : player.transform;
        StartCoroutine(AttractToPlayer(target));
    }

    private IEnumerator AttractToPlayer(Transform playerTransform)
    {
        while (Vector3.Distance(transform.position, playerTransform.position) > minDistanceForDestroy)
        {
            // Interpolación para mover la moneda hacia el jugador
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * attractSpeed * Time.deltaTime;
            yield return null;
        }
        
        // Actualiza el inventario del UIManager antes de destruir la moneda.
        FindObjectOfType<UIManager>().AgregarMonedas(1);
        Destroy(gameObject);
    }
}
