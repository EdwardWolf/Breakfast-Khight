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
    private float velocidadOriginal; // Para almacenar la velocidad original del jugador
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
        // Mover en la dirección en la que el jugador está mirando
        Vector3 moveDirection = transform.forward * direccion.z + transform.right * direccion.x;
        transform.Translate(moveDirection * stats.velocidadMovimiento * Time.deltaTime, Space.World);
    }

    public override void ActivarAtaque()
    {
        if (!escudoActivo)
        {
            // Ataque simple
            Debug.Log("Caballero Ligero está atacando");
            animator.SetTrigger("Ataque"); // Activar la animación de ataque
            StartCoroutine(ActivarColliderTemporalmente());
        }
    }

    public override void ActivarAtaqueCargado()
    {
        if (!escudoActivo)
        {
            // Ataque cargado
            Debug.Log("Caballero Ligero está realizando un ataque cargado");
            animator.SetTrigger("AtaqueCargado"); // Activar la animación de ataque cargado
            StartCoroutine(ActivarColliderTemporalmente());
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
            escudoActivo = true;
            stats.velocidadMovimiento /= 2; // Reducir la velocidad a la mitad
            Debug.Log("Escudo activado.");
            animator.SetTrigger("EscudoActivado"); // Activar la animación de escudo activado
        }
    }

    public void DesactivarEscudo()
    {
        if (escudoActivo)
        {
            escudoActivo = false;
            stats.velocidadMovimiento = velocidadOriginal; // Restaurar la velocidad original
            Debug.Log("Escudo desactivado.");
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

    private IEnumerator ActivarColliderTemporalmente()
    {
        hijogolpe.enabled = true;
        yield return new WaitForSeconds(0.5f); // Ajusta el tiempo según la duración del golpe
        hijogolpe.enabled = false;
    }
}
