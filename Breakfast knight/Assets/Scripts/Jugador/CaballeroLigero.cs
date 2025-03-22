using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CaballeroLigero : Jugador
{
    //private bool escudoActivo = false;
    public Collider Escudo; // Referencia al collider del escudo
    private GameInputs playerInputActions;
    public Animator animator; // Referencia al componente Animator
    //public Animator armas; // Referencia al componente Animator
    private float attackLayerWeight = 0f;
    private float escudoLayerWeight = 0f;
    private float chargeLayerWeight = 0f;
    private float cargadoLayerWeight = 0f;
    public float attackTransitionSpeed = 5f; // Velocidad a la que el layer de ataque cambia su peso
    private bool isAttacking = false;
    private void Awake()
    {

        playerInputActions = new GameInputs();
        Escudo.enabled = false;
        animator = GetComponentInChildren<Animator>(); // Obtener el componente Animator del hijo

    }
    private void Update()
    {
        cargaBarra.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
                 camara.transform.rotation * Vector3.up);

        GirarHaciaMouse();
        
        Vector3 movimiento = PlayerController.GetMoveInput();
        Mover(movimiento);

        if (movimiento != Vector3.zero)
        {
            animator.SetBool("Caminar",true); // Activar la animación de caminar

        }
        else
        {
            animator.SetBool("Caminar", false); // Desactivar la animación de caminar
        }

        if (isCharging)
        {
            chargeLayerWeight = 1f;
            animator.SetLayerWeight(3, chargeLayerWeight);
            animator.SetBool("Carga", true);

            chargeTime += Time.deltaTime;
            if (cargaBarra != null)
            {
                cargaBarra.fillAmount = chargeTime / maxChargeTime;
            }

            if (chargeTime >= maxChargeTime)
            {
                chargeLayerWeight = 0f;
                animator.SetLayerWeight(3,chargeLayerWeight);
                animator.SetBool("Carga", false);
                AtaqueCargadoArea();
                animator.SetTrigger("AtaqueCargado");
                cargadoLayerWeight = 1f;
                animator.SetLayerWeight(4,cargadoLayerWeight);
                isCharging = false;
                chargeTime = 0f;
                if (cargaBarra != null)
                {
                    cargaBarra.fillAmount = 0f;
                }
            }
        }
        else
        {
            chargeLayerWeight = 0f;
            animator.SetLayerWeight(3, chargeLayerWeight);
            animator.SetBool("Carga", false);
        }

        //RegenerarResistenciaEscudo();

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

        // Ajustar el peso del Attack Layer (index 1)
        animator.SetLayerWeight(1, attackLayerWeight);
        animator.SetLayerWeight(2, escudoLayerWeight);
        animator.SetLayerWeight(3, chargeLayerWeight);
        animator.SetLayerWeight(4, cargadoLayerWeight);
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
        if (!escudoActivo && !aturdidoBala && !isAttacking)
        {
            if (animator != null)
            {
                isAttacking = true; // Iniciar la animación de ataque
                //attackLayerWeight = Mathf.Lerp(attackLayerWeight, 1f, Time.deltaTime * attackTransitionSpeed);
                attackLayerWeight = 1f;

                animator.SetTrigger("Ataque"); // Activar la animación de ataque
                StartCoroutine(WaitAndResetAttackLayerWeight(1f));
            }
            StartCoroutine(ActivarColliderEspada());
        }
        else
        {
            isAttacking = false;
            attackLayerWeight = 0f;
            //attackLayerWeight = Mathf.Lerp(attackLayerWeight, 0f, Time.deltaTime * attackTransitionSpeed);
        }
        animator.SetLayerWeight(1, attackLayerWeight); // Ajustar el peso del Attack Layer (index 1)

        // Aquí puedes revisar si la animación de ataque ha terminado y resetear el estado
        if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1f && !animator.IsInTransition(1))
        {
            isAttacking = false;
        }
    }
    //Verifica el estatus de la trancision de la animacion
    //private IEnumerator WaitAndResetAttackLayerWeight()
    //{
    //    // Esperar a que la animación de ataque termine
    //    while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f || animator.IsInTransition(1))
    //    {
    //        yield return null;
    //    }
    //    attackLayerWeight = 0f;
    //}
    private IEnumerator WaitAndResetAttackLayerWeight(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        attackLayerWeight = 0f;
    }

    public override void ActivarInteraction()
    {
        // Interactuar
        Debug.Log("Caballero Ligero está interactuando");
    }

    private void ActivarEscudo()
    {
        if (!escudoActivo && !aturdidoBala)
        {
            Debug.Log("escudo Activo");
            Escudo.enabled = true;
            escudoActivo = true;
            _velocidadMovimiento /= 2; // Reducir la velocidad a la mitad
            animator.SetTrigger("EscudoActivado"); // Activar la animación de escudo activado
            escudoLayerWeight = 1f;
            animator.SetLayerWeight(2, escudoLayerWeight); // Ajustar el peso de Defensa Layer (index 1)
        }
    }

    private void DesactivarEscudo()
    {
        if (escudoActivo && !aturdidoBala)
        {
            Debug.Log("Escudo desactivado");
            //armas.SetTrigger("EscudoDesactivado");
            Escudo.enabled = false;
            escudoActivo = false;
            _velocidadMovimiento = stats.velocidadMovimiento; // Restaurar la velocidad original
            animator.SetTrigger("EscudoDesactivado"); // Activar la animación de escudo desactivado
            escudoLayerWeight = 0f;
            animator.SetLayerWeight(2, escudoLayerWeight); // Ajustar el peso de Defensa Layer (index 1)

        }
    }


    private IEnumerator ActivarColliderEspada()
    {
        armaCollider.enabled = true;
        yield return new WaitForSeconds(0.5f); // Ajusta el tiempo según la duración del golpe
        armaCollider.enabled = false;
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

    private IEnumerator FinalizarAtaque()
    {
        // Esperar la duración de la animación de ataque
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false; // Finalizar la animación de ataque
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        if (!juegoPausado)
        {
            ActivarAtaque();
        }
    }
}

