using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class Aderezo : MonoBehaviour
{
    public float interactionDistance = 3f; // Distancia de interacción
    public float interactionTime = 2f; // Tiempo necesario para completar la interacción
    private bool isInteracting = false;
    public float interactionTimer = 0f; // Tiempo transcurrido de la interacción
    public Jugador jugador;
    public Enemigo enemigo;
    private GameInputs playerInputActions;
    public Image interactionProgressBar; // Referencia a la barra de progreso
    private Camera camara; // Añadir referencia a la cámara

    private static Enemigo enemigoInteractuando; // Variable estática para almacenar el enemigo que está interactuando

    private void Start()
    {
        gameObject.SetActive(true); // Activar el objeto instanciado
    }

    private void Awake()
    {
        camara = Camera.main; // Obtener la cámara principal    
        playerInputActions = new GameInputs();
    }

    private void OnEnable()
    {
        playerInputActions.Player.Interact.performed += OnInteractPerformed;
        playerInputActions.Player.Interact.canceled += OnInteractCanceled;
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Interact.performed -= OnInteractPerformed;
        playerInputActions.Player.Interact.canceled -= OnInteractCanceled;
        playerInputActions.Disable();
    }

    public void Update()
    {
        interactionProgressBar.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
        camara.transform.rotation * Vector3.up);

        if (jugador != null && Vector3.Distance(transform.position, jugador.transform.position) <= interactionDistance)
        {
            if (isInteracting)
            {
                interactionTimer += Time.deltaTime;
                UpdateProgressBar(interactionTimer / interactionTime);

                if (interactionTimer >= interactionTime)
                {
                    RealizarInteraccionJugador();
                    isInteracting = false;
                    UpdateProgressBar(0f);
                }
            }
            else
            {
                interactionTimer = 0f;
                UpdateProgressBar(0f);
            }
        }
        else if (enemigo != null && Vector3.Distance(transform.position, enemigo.transform.position) <= interactionDistance)
        {
            if (isInteracting)
            {
                interactionTimer += Time.deltaTime;
                UpdateProgressBar(interactionTimer / interactionTime);

                if (interactionTimer >= interactionTime)
                {
                    RealizarInteraccionEnemigo();
                    isInteracting = false;
                    enemigoInteractuando = null; // Liberar la referencia del enemigo interactuando
                    UpdateProgressBar(0f);
                }
            }
            else
            {
                interactionTimer = 0f;
                UpdateProgressBar(0f);
            }
        }
        else
        {
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

    private void RealizarInteraccionJugador()
    {
        IncrementarAtaqueJugador();
        IncrementarVelocidadJugador();
    }

    private void RealizarInteraccionEnemigo()
    {
        IncrementarAtaqueEnemigo();
        IncrementarVelocidadEnemigo();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (jugador != null && Vector3.Distance(transform.position, jugador.transform.position) <= interactionDistance)
        {
            isInteracting = true;
            interactionTimer = 0f;
        }
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        isInteracting = false;
        interactionTimer = 0f;
        UpdateProgressBar(0f);
    }

    private void UpdateProgressBar(float progress)
    {
        if (interactionProgressBar != null)
        {
            interactionProgressBar.fillAmount = progress;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugador = other.GetComponent<Jugador>();
        }
        if (other.CompareTag("Enemigo"))
        {
            if (enemigoInteractuando == null)
            {
                enemigo = other.GetComponent<Enemigo>();
                enemigoInteractuando = enemigo; // Asignar la referencia del enemigo interactuando
                isInteracting = true; // Iniciar la interacción automáticamente para el enemigo
                interactionTimer = 0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugador = null;
            UpdateProgressBar(0f);
        }
        if (other.CompareTag("Enemigo"))
        {
            if (enemigo == other.GetComponent<Enemigo>())
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
