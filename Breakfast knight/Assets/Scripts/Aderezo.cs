using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Aderezo : MonoBehaviour
{
    public float interactionDistance = 3f; // Distancia de interacción
    public float interactionTime = 2f; // Tiempo necesario para completar la interacción
    public float incrementoAtaque = 10f; // Incremento de ataque
    public int cantidadGolpes = 5; // Cantidad de golpes antes de restablecer el ataque
    private bool isInteracting = false;
    private float interactionTimer = 0f;
    private Jugador jugador;
    private GameInputs playerInputActions;
    public Image interactionProgressBar; // Referencia a la barra de progreso
    private Camera camara; // Añadir referencia a la cámara

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

    private void Update()
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
                    IncrementarAtaqueJugador();
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
        else
        {
            UpdateProgressBar(0f);
        }
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

    private void IncrementarAtaqueJugador()
    {
        if (jugador != null)
        {
            jugador.IncrementarAtaqueTemporal(incrementoAtaque, cantidadGolpes);
            jugador.uiManager.MostrarIncrementoAtaque(incrementoAtaque, cantidadGolpes); // Actualizar la UI
            gameObject.SetActive(false); // Desactivar el objeto instanciado
            Debug.Log("Ataque del jugador incrementado temporalmente");
        }
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugador = null;
            UpdateProgressBar(0f);
        }
    }
}
