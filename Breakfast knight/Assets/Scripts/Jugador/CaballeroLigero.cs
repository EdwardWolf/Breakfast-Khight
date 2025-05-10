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
    public bool isAttacking = false;
    public ParticleSystem carga;

    [SerializeField] private Material materialEscudoActivo; // Material del escudo activo
    [SerializeField] private Material materialEscudoNormal; // Material del escudo normal

    private void Awake()
    {
        playerInputActions = new GameInputs();
        Escudo.enabled = false;
        animator = GetComponentInChildren<Animator>(); // Obtener el componente Animator del hijo
    }

    private void Update()
    {
        // Actualizar el valor de ataque del arma equipada
        if (armas[armaActual] != null)
        {
            ataque = armas[armaActual].daño;
        }
        cargaBarra.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
                 camara.transform.rotation * Vector3.up);

        GirarHaciaMouse();

        Vector3 movimiento = PlayerController.GetMoveInput();
        Mover(movimiento);

        if (movimiento != Vector3.zero)
        {
            animator.SetBool("Caminar", true); // Activar la animación de caminar
        }
        else
        {
            animator.SetBool("Caminar", false); // Desactivar la animación de caminar
        }

        if (isCharging)
        {
            if (!carga.isPlaying)
            {
                carga.Play();
            }
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
                animator.SetLayerWeight(3, chargeLayerWeight);
                animator.SetBool("Carga", false);
                AtaqueCargadoArea();
                animator.SetTrigger("AtaqueCargado");
                cargadoLayerWeight = 1f;
                animator.SetLayerWeight(4, cargadoLayerWeight);
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
            if (carga.isPlaying)
            {
                carga.Stop();
            }
            chargeLayerWeight = 0f;
            animator.SetLayerWeight(3, chargeLayerWeight);
            animator.SetBool("Carga", false);
        }

        // Verificar si la resistencia del escudo es cero y desactivar el escudo
        if (resistenciaEscudoActual <= 0 && escudoActivo)
        {
            DesactivarEscudo();
        }

        if (PlayerController.Interaccion())
        {
            ActivarInteraction();
        }
        if (PlayerController.Shield() && resistenciaEscudoActual > 0)
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

        // Debugging: Imprimir valores del Animator
        //Debug.Log($"isAttacking: {isAttacking}");
        //Debug.Log($"Attack Layer Weight: {animator.GetLayerWeight(1)}");
        //Debug.Log($"Current State: {animator.GetCurrentAnimatorStateInfo(1).normalizedTime}");
        //Debug.Log($"Is In Transition: {animator.IsInTransition(1)}");
    
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
        if (!escudoActivo && !aturdidoBala) // Eliminamos la verificación de isAttacking
        {
            attackLayerWeight = 1f;

            if (animator != null)
            {
                switch (armaActual)
                {
                    case 0:
                        animator.SetTrigger("Cuchara");
                        break;
                    case 1:
                        animator.SetTrigger("Cuchillo");
                        break;
                    case 2:
                        animator.SetTrigger("Tenedor");
                        break;
                }
            }

            StartCoroutine(ActivarColliderEspada());
        }
    }

    //private IEnumerator WaitAndResetAttackState()
    //{
    //    // Esperar hasta que la animación de ataque termine
    //    while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f || animator.IsInTransition(1))
    //    {
    //        yield return null; // Esperar al siguiente frame
    //    }

    //    // Restablecer el estado de ataque
    //    isAttacking = false;
    //    attackLayerWeight = 0f;
    //    animator.SetLayerWeight(1, attackLayerWeight);
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
            animator.SetBool("Escudo", true); // Activar la animación de escudo activado
            escudoLayerWeight = 1f;
            animator.SetLayerWeight(2, escudoLayerWeight); // Ajustar el peso de Defensa Layer (index 1)

            // Cambiar el material del escudo
            EscudoR.material = materialEscudoActivo;
        }
    }

    private void DesactivarEscudo()
    {
        if (escudoActivo)
        {
            Debug.Log("Escudo desactivado");
            Escudo.enabled = false;
            escudoActivo = false;
            _velocidadMovimiento = stats.velocidadMovimiento; // Restaurar la velocidad original
            animator.SetBool("Escudo", false); // Desactivar la animación de escudo activado
            escudoLayerWeight = 0f;
            animator.SetLayerWeight(2, escudoLayerWeight); // Ajustar el peso de Defensa Layer (index 1)

            // Restaurar el material del escudo
            EscudoR.material = materialEscudoNormal;
        }
    }

    private IEnumerator ActivarColliderEspada()
    {
        armaCollider.enabled = true;
        yield return new WaitForSeconds(1f); // Ajusta el tiempo según la duración del golpe
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
