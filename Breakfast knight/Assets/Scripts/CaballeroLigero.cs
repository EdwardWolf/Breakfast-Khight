using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaballeroLigero : Jugador
{
    private bool escudoActivo = false;
    private bool isChargingAttack = false;
    private float attackChargeTime = 0f;
    private float requiredChargeTime = 2f; // Tiempo necesario para un ataque cargado
    public Animator animator; // Referencia al componente Animator
    public Collider hijogolpe; // Referencia al collider del golpe
    
    
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
            StartCoroutine(ActivarColliderTemporalmente());
        }
    }

    public override void ActivarAtaqueCargado()
    {
        if (!escudoActivo)
        {
            // Ataque cargado
            Debug.Log("Caballero Ligero est� realizando un ataque cargado");
            animator.SetTrigger("AtaqueCargado"); // Activar la animaci�n de ataque cargado
            StartCoroutine(ActivarColliderTemporalmente());
        }
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

    private IEnumerator ActivarColliderTemporalmente()
    {
        hijogolpe.enabled = true;
        yield return new WaitForSeconds(0.5f); // Ajusta el tiempo seg�n la duraci�n del golpe
        hijogolpe.enabled = false;
    }
}
