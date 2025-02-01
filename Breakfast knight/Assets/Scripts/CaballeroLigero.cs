using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CaballeroLigero : Jugador
{
    private bool escudoActivo = false;
    public Collider Espada; // Referencia al collider del golpe
    public Collider Escudo; // Referencia al collider del escudo
    private GameInputs playerInputActions;
    public Animator animator; // Referencia al componente Animator
    public Animator armas; // Referencia al componente Animator

    private void Awake()
    {
        playerInputActions = new GameInputs();
        Escudo.enabled = false;

    }
    private void Update()
    {
        GirarHaciaMouse();
        Vector3 movimiento = PlayerController.GetMoveInput();
        Mover(movimiento);

        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            if (cargaBarra != null)
            {
                cargaBarra.fillAmount = chargeTime / maxChargeTime;
            }

            if (chargeTime >= maxChargeTime)
            {
                AtaqueCargadoArea();
                isCharging = false;
                chargeTime = 0f;
                if (cargaBarra != null)
                {
                    cargaBarra.fillAmount = 0f;
                }
            }
        }

        RegenerarResistenciaEscudo();

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

    //Activar de regreso para el movimiento en funcion a la direccion
    //public override void Mover(Vector3 direccion)
    //{
    //    // Mover en la dirección en la que el jugador está mirando
    //    Vector3 moveDirection = transform.forward * direccion.z + transform.right * direccion.x;
    //    transform.Translate(moveDirection * _velocidadMovimiento * Time.deltaTime, Space.World);
    //}

    public override void ActivarAtaque()
    {
        if (!escudoActivo)
        {
            armas.SetTrigger("Ataque");
            // Ataque simple
            animator.SetTrigger("Ataque"); // Activar la animación de ataque
            StartCoroutine(ActivarColliderEspada());
        }
    }

    public override void ActivarInteraction()
    {
        // Interactuar
        Debug.Log("Caballero Ligero está interactuando");
    }

    public void ActivarEscudo()
    {
        if (!escudoActivo)
        {
            armas.SetTrigger("EscudoActivado");
            Escudo.enabled = true;
            escudoActivo = true;
            _velocidadMovimiento /= 2; // Reducir la velocidad a la mitad
            animator.SetTrigger("EscudoActivado"); // Activar la animación de escudo activado
        }
    }

    public void DesactivarEscudo()
    {
        if (escudoActivo)
        {
            armas.SetTrigger("EscudoDesactivado");
            Escudo.enabled = false;
            escudoActivo = false;
            _velocidadMovimiento = stats.velocidadMovimiento; // Restaurar la velocidad original
            animator.SetTrigger("EscudoDesactivado"); // Activar la animación de escudo desactivado
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
        yield return new WaitForSeconds(0.5f); // Ajusta el tiempo según la duración del golpe
        Espada.enabled = false;
    }
}

