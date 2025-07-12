using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Aderezo : MonoBehaviour
{
    public float interactionDistance = 2f; // Distancia de interacción
    public float interactionTime = 2f; // Tiempo necesario para completar la interacción
    private bool isInteracting = false;
    public float interactionTimer = 0f; // Tiempo transcurrido de la interacción
    public Jugador jugador;
    public Enemigo enemigo;
    public Image interactionProgressBar; // Referencia a la barra de progreso
    private Camera camara; // Añadir referencia a la cámara
    [SerializeField] private Sprite AderezoUI; // Prefab del objeto a instanciar
    public Image indicadorInteraccion; // Asigna esta imagen desde el inspector
    private static Enemigo enemigoInteractuando; // Variable estática para almacenar el enemigo que está interactuando

    [Header("Configuración de duración")]
    [SerializeField] protected bool tieneDuracion = false;
    [SerializeField] protected float duracionEfecto = 5f;

    private void Start()
    {
        gameObject.SetActive(true);
        interactionProgressBar = transform.Find("Canvas/Barra").GetComponent<Image>();

        if (interactionProgressBar != null)
        {
            interactionProgressBar.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("No se encontró el interactionProgressBar en la jerarquía.");
        }

        if (indicadorInteraccion != null)
        {
            indicadorInteraccion.gameObject.SetActive(false); // Oculta el indicador al inicio
        }
    }

    protected virtual void Awake()
    {
        camara = Camera.main; // Obtener la cámara principal    
    }

    protected virtual void Update()
    {
        indicadorInteraccion.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
                     camara.transform.rotation * Vector3.up);

        interactionProgressBar.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
        camara.transform.rotation * Vector3.up);

        // Interacción automática con el jugador
        if (jugador != null && Vector3.Distance(transform.position, jugador.transform.position) <= interactionDistance)
        {
            isInteracting = true;
            interactionTimer += Time.deltaTime;
            UpdateProgressBar(interactionTimer / interactionTime);

            if (interactionTimer >= interactionTime)
            {
                RealizarInteraccionJugador();
                isInteracting = false;
                interactionTimer = 0f;
                UpdateProgressBar(0f);
            }
        }
        // Interacción automática con el enemigo
        else if (enemigo != null && Vector3.Distance(transform.position, enemigo.transform.position) <= interactionDistance)
        {
            isInteracting = true;
            interactionTimer += Time.deltaTime;
            UpdateProgressBar(interactionTimer / interactionTime);

            if (interactionTimer >= interactionTime)
            {
                RealizarInteraccionEnemigo();
                isInteracting = false;
                enemigoInteractuando = null; // Liberar la referencia del enemigo interactuando
                interactionTimer = 0f;
                UpdateProgressBar(0f);
            }
        }
        else
        {
            isInteracting = false;
            interactionTimer = 0f;
            UpdateProgressBar(0f);
        }
    }

    protected virtual void IncrementarAtaqueJugador()
    {
        // Este método será sobrescrito por AderezoAtaque
    }

    protected virtual void IncrementarAtaqueEnemigo()
    {
        // Este método será sobrescrito por AderezoAtaque
    }

    protected virtual void IncrementarVelocidadJugador()
    {
        // Este método será sobrescrito por AderezoVelocidad
    }

    protected virtual void IncrementarVelocidadEnemigo()
    {
        // Este método será sobrescrito por AderezoVelocidad
    }
    protected virtual void ReuperarSaludJugador()
    {
        // Este método será sobrescrito por AderezoSalud
    }
    protected virtual void ReuperarSaludEnemigo()
    {
        // Este método será sobrescrito por AderezoSalud
    }

    private void RealizarInteraccionJugador()
    {
        IncrementarAtaqueJugador();
        IncrementarVelocidadJugador();
        ReuperarSaludJugador();
    }

    private void RealizarInteraccionEnemigo()
    {
        IncrementarAtaqueEnemigo();
        IncrementarVelocidadEnemigo();
        ReuperarSaludEnemigo();
    }

    private void UpdateProgressBar(float progress)
    {
        if (interactionProgressBar != null)
        {
            interactionProgressBar.fillAmount = progress;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            jugador = collision.gameObject.GetComponent<Jugador>();
            if (indicadorInteraccion != null)
                indicadorInteraccion.gameObject.SetActive(true); // Muestra la imagen
        }
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            if (enemigoInteractuando == null)
            {
                enemigo = collision.gameObject.GetComponent<Enemigo>();
                enemigoInteractuando = enemigo; // Asignar la referencia del enemigo interactuando
                isInteracting = true; // Iniciar la interacción automáticamente para el enemigo
                interactionTimer = 0f;
            }
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            jugador = null;
            UpdateProgressBar(0f);
            if (indicadorInteraccion != null)
                indicadorInteraccion.gameObject.SetActive(false); // Oculta la imagen
        }
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            if (enemigo == collision.gameObject.GetComponent<Enemigo>())
            {
                enemigo = null;
                enemigoInteractuando = null; // Liberar la referencia del enemigo interactuando
                isInteracting = false; // Detener la interacción cuando el enemigo sale del rango
                interactionTimer = 0f;
                UpdateProgressBar(0f);
            }
        }
    }
}
