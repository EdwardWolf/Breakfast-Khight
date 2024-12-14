using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class CaballeroLigero : Jugador
{
    private bool escudoActivo = false;
    private bool isChargingAttack = false;
    private float attackChargeTime = 0f;
    private float requiredChargeTime = 2f; // Tiempo necesario para un ataque cargado
    public Animator animator; // Referencia al componente Animator
    public Collider Espada; // Referencia al collider del golpe
    public Collider Escudo; // Referencia al collider del escudo
    private GameInputs playerInputActions;

    private void Awake()
    {
        playerInputActions = new GameInputs();

        Escudo.enabled = false;
    }

    private void OnEnable()
    {
        playerInputActions.Player.AtaqueCargado.started += OnChargeAttackStarted;
        playerInputActions.Player.AtaqueCargado.canceled += OnChargeAttackCanceled;
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.AtaqueCargado.started -= OnChargeAttackStarted;
        playerInputActions.Player.AtaqueCargado.canceled -= OnChargeAttackCanceled;
        playerInputActions.Disable();
    }

    private void Update()
    {
        GirarHaciaMouse();
        Vector3 movimiento = PlayerController.GetMoveInput();
        Mover(movimiento);

        if (!escudoActivo)
        {
            if (PlayerController.IsAttackPressed())
            {
                if (!isChargingAttack)
                {
                    isChargingAttack = true;
                    attackChargeTime = 0f;
                }
                else
                {
                    attackChargeTime += Time.deltaTime;
                    if (attackChargeTime >= requiredChargeTime)
                    {
                        ActivarAtaqueCargado();
                        isChargingAttack = false;
                    }
                }
            }
            else if (isChargingAttack)
            {
                ActivarAtaque();
                isChargingAttack = false;
            }
        }

        if (PlayerController.Interaccion())
        {
            ActivarInteraction();
        }
        if (PlayerController.Shield())
        {
            ActivarEscudo();
        }
        else
        {
            DesactivarEscudo();
        }
    }

    public override void Mover(Vector3 direccion)
    {
        // Mover en la direcci�n en la que el jugador est� mirando
        Vector3 moveDirection = transform.forward * direccion.z + transform.right * direccion.x;
        transform.Translate(moveDirection * _velocidadMovimiento * Time.deltaTime, Space.World);
    }

    public override void ActivarAtaque()
    {
        if (!escudoActivo)
        {
            // Ataque simple
            Debug.Log("Caballero Ligero est� atacando");
            animator.SetTrigger("Ataque"); // Activar la animaci�n de ataque
            StartCoroutine(ActivarColliderEspada());
        }
    }

    private void OnChargeAttackStarted(InputAction.CallbackContext context)
    {
        IniciarCarga();
    }

    private void OnChargeAttackCanceled(InputAction.CallbackContext context)
    {
        CancelarCarga();
    }
    public override void ActivarAtaqueCargado()
    {
        Debug.Log("Caballero Ligero está realizando un ataque cargado de área");
        AtaqueCargadoArea();
    }

    public override void ActivarInteraction()
    {
        // Interactuar
        Debug.Log("Caballero Ligero est� interactuando");
    }

    public void ActivarEscudo()
    {
        if (!escudoActivo)
        {
            Escudo.enabled = true;
            escudoActivo = true;
            _velocidadMovimiento /= 2; // Reducir la velocidad a la mitad
            Debug.Log("Escudo activado.");
            animator.SetTrigger("EscudoActivado"); // Activar la animaci�n de escudo activado
        }
    }

    public void DesactivarEscudo()
    {
        if (escudoActivo)
        {
            Escudo.enabled = false;
            escudoActivo = false;
            _velocidadMovimiento = stats.velocidadMovimiento; // Restaurar la velocidad original
            Debug.Log("Escudo desactivado.");
            animator.SetTrigger("EscudoDesactivado"); // Activar la animaci�n de escudo desactivado
        }
    }

    private void GirarHaciaMouse()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (playerPlane.Raycast(ray, out float hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private IEnumerator ActivarColliderEspada()
    {
        Espada.enabled = true;
        yield return new WaitForSeconds(0.5f); // Ajusta el tiempo seg�n la duraci�n del golpe
        Espada.enabled = false;
    }

}
