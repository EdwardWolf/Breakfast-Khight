using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI; // Asegúrate de incluir esta librería para trabajar con UI


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

    [SerializeField] private Material materialEscudoCompleto; // Material cuando resistencia está a 3/3
    [SerializeField] private Material materialEscudoMedio;    // Material cuando resistencia está a 2/3
    [SerializeField] private Material materialEscudoBajo;     // Material cuando resistencia está a 1/3

    public Image ataqueCargadoBarra; // Referencia a la barra de carga del ataque cargado
    public ParticleSystem particulasResistenciaEscudo;

    // Añadir esta variable para el sistema de partículas del escudo roto
    [SerializeField] private ParticleSystem particulasEscudoRoto;

    private Coroutine coroutinaParticulasEscudo;

    // Añadir esta variable para rastrear el valor anterior de resistencia
    private float resistenciaEscudoAnterior;

    private void Awake()
    {
        playerInputActions = new GameInputs();
        Escudo.enabled = false;
        animator = GetComponentInChildren<Animator>(); // Obtener el componente Animator del hijo
    }

    protected override void Start()
    {
        base.Start();
        escudoCollider = Escudo; // Asigna el collider del escudo de la clase derivada a la base
        resistenciaEscudoAnterior = resistenciaEscudoActual; // Inicializar el valor anterior

        // Iniciar la comprobación periódica del material durante la regeneración
        StartCoroutine(ComprobarRegeneracionEscudo());
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

        //Camindado de jugador
        Vector3 movimiento = PlayerController.GetMoveInput();
        Mover(movimiento);


        if (movimiento != Vector3.zero)
        {
            Vector3 movimientoLocal = transform.InverseTransformDirection(movimiento);
            bool vaAtras = movimientoLocal.z < -0.1f;
            bool vaFrente = movimientoLocal.z > 0.1f;
            bool vaIzquierda = movimientoLocal.x < -0.1f;
            bool vaDerecha = movimientoLocal.x > 0.1f;

            animator.SetBool("Retroceder", vaAtras);
            animator.SetBool("Caminar", vaFrente);
            animator.SetBool("LateralIzquierda", vaIzquierda);
            animator.SetBool("LateralDerecha", vaDerecha);

            // Puedes ajustar los LayerWeight según tu lógica, por ejemplo:
            if(vaFrente && vaIzquierda)
            {
                animator.SetLayerWeight(5, 1f);
                animator.SetLayerWeight(0, 0f);
            }
            else

              if (vaFrente && vaDerecha)
            {
                animator.SetLayerWeight(5, 0f);
                animator.SetLayerWeight(0, 1f);
            }

            if (vaAtras)
            {
                animator.SetLayerWeight(5, 1f);
                animator.SetLayerWeight(0, 0f);
            }
            else
            {
                animator.SetLayerWeight(5, 0f);
                animator.SetLayerWeight(0, 1f);
            }
        }
        else
        {
            animator.SetBool("Caminar", false);
            animator.SetBool("Retroceder", false);
            animator.SetBool("LateralIzquierda", false);
            animator.SetBool("LateralDerecha", false);
            animator.SetLayerWeight(5, 0f);
            animator.SetLayerWeight(0, 1f);
        }

        //Ataque Cargado
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

            // Actualizar la barra de carga del ataque cargado
            if (ataqueCargadoBarra != null)
            {
                ataqueCargadoBarra.fillAmount = chargeTime / maxChargeTime;
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

                // Reiniciar la barra de carga
                if (ataqueCargadoBarra != null)
                {
                    ataqueCargadoBarra.fillAmount = 0f;
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

            // Reiniciar la barra de carga si se cancela
            if (ataqueCargadoBarra != null)
            {
                ataqueCargadoBarra.fillAmount = 0f;
            }
        }
        // Desactivar el objeto del escudo si la resistencia es 0 o menos
        if (resistenciaEscudoActual <= 0)
        {
            if (Escudo.gameObject.activeSelf)
                Escudo.gameObject.SetActive(false);
            escudoActivo = false;
            if (playerInputActions != null)
                playerInputActions.Player.Dash.Disable();

            // Desactivar la animación del escudo
            escudoLayerWeight = 0f;
            animator.SetLayerWeight(2, escudoLayerWeight);

            // Activar partículas del escudo roto si existe el sistema
            if (particulasEscudoRoto != null)
            {
                particulasEscudoRoto.transform.position = Escudo.transform.position;
                particulasEscudoRoto.Play();
            }
        }
        // Activar el objeto del escudo SOLO cuando la resistencia esté al máximo
        else if (resistenciaEscudoActual >= stats.resistenciaEscudo)
        {
            if (!Escudo.gameObject.activeSelf)
                Escudo.gameObject.SetActive(true);
        }

        // La animación se puede hacer siempre por input
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
    
        // Detectar cambios en la resistencia del escudo (regeneración)
        if (escudoActivo && resistenciaEscudoActual > resistenciaEscudoAnterior)
        {
            // El escudo está regenerándose mientras está activo
            ActualizarMaterialEscudo();
        }
        
        // Guardar el valor actual para la siguiente comprobación
        resistenciaEscudoAnterior = resistenciaEscudoActual;
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
            // Activar el TrailRenderer del arma
            if (armas[armaActual] != null)
            {
                armas[armaActual].ActivarTrail();
            }
            StartCoroutine(ActivarColliderEspada());
            StartCoroutine(DesactivarTrailDespuesDeAtaque());
        }
    }

    private IEnumerator DesactivarTrailDespuesDeAtaque()
    {
        // Esperar hasta que la animación de ataque termine
        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f || animator.IsInTransition(1))
        {
            yield return null; // Esperar al siguiente frame
        }

        // Desactivar el TrailRenderer del arma
        if (armas[armaActual] != null)
        {
            armas[armaActual].DesactivarTrail();
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
            animator.SetBool("Escudo", true); // Usar bool en vez de trigger
            escudoLayerWeight = 1f;
            animator.SetLayerWeight(2, escudoLayerWeight);

            // Actualizar el material del escudo según el nivel actual
            ActualizarMaterialEscudo();

            // Habilitar el input del dash
            if (playerInputActions != null)
                playerInputActions.Player.Dash.Enable();
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
            animator.SetBool("Escudo", false); // Usar bool en vez de trigger

            // No necesitamos actualizar el material aquí, ya que el escudo está desactivado

            // Deshabilitar el input del dash
            if (playerInputActions != null)
                playerInputActions.Player.Dash.Disable();
            escudoLayerWeight = 0f;
            animator.SetLayerWeight(2, escudoLayerWeight);
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

    // Modificar el método override para manejar la reducción de resistencia del escudo
    public override void ReducirResistenciaEscudo(float cantidad)
    {
        // Guardar el valor anterior para comprobar si se rompe
        float resistenciaAnterior = resistenciaEscudoActual;
        
        // Llamar al método base para mantener la funcionalidad original
        base.ReducirResistenciaEscudo(cantidad);
        
        // Actualizar el material del escudo según el nivel actual
        ActualizarMaterialEscudo();
        
        // Comprobar si el escudo se ha roto en esta reducción de resistencia
        if (resistenciaAnterior > 0 && resistenciaEscudoActual <= 0)
        {
            // El escudo se acaba de romper - activar efecto
            ActivarEfectoEscudoRoto();
        }
        
        // Activa las partículas normales si existe el sistema
        if (particulasResistenciaEscudo != null)
        {
            // Si ya hay una coroutina activa, detenerla
            if (coroutinaParticulasEscudo != null)
                StopCoroutine(coroutinaParticulasEscudo);
                
            // Iniciar nueva coroutina para controlar las partículas
            coroutinaParticulasEscudo = StartCoroutine(MostrarParticulasResistenciaEscudo());
        }
    }
    private IEnumerator MostrarParticulasResistenciaEscudo()
    {
        if (particulasResistenciaEscudo != null)
        {
            particulasResistenciaEscudo.Play();
            yield return new WaitForSeconds(particulasResistenciaEscudo.main.duration);
            particulasResistenciaEscudo.Stop();
        }
        else
        {
            yield return null;
        }
    }

    // Añadir este método para activar el efecto cuando el escudo se rompe
    private void ActivarEfectoEscudoRoto()
    {
        if (particulasEscudoRoto != null)
        {
            // Posicionamos las partículas en la posición del escudo
            particulasEscudoRoto.transform.position = Escudo.transform.position;
            
            // Reproducimos las partículas
            particulasEscudoRoto.Play();
            
            // Opcional: Reproducir sonido de escudo rompiéndose
            if (GetComponent<AudioSource>() != null)
            {
                AudioSource audio = GetComponent<AudioSource>();
                // Asumiendo que tienes un clip de audio para el escudo rompiéndose
                AudioClip sonidoRotura = Resources.Load<AudioClip>("Sonidos/EscudoRoto");
                if (sonidoRotura != null)
                    audio.PlayOneShot(sonidoRotura);
            }
            
            // Opcional: Añadir efecto de cámara o feedback visual adicional
            StartCoroutine(DesactivarParticulasEscudoRoto());
        }
    }

    // Corrutina para desactivar las partículas del escudo roto después de su reproducción
    private IEnumerator DesactivarParticulasEscudoRoto()
    {
        // Esperar a que terminen de reproducirse las partículas
        yield return new WaitForSeconds(particulasEscudoRoto.main.duration);
        
        // Detener la emisión si aún está activa
        if (particulasEscudoRoto.isPlaying)
        {
            particulasEscudoRoto.Stop();
        }
    }

    // Añadir este método después de ReducirResistenciaEscudo
    private void ActualizarMaterialEscudo()
    {
        float resistenciaMaxima = stats.resistenciaEscudo;
        float umbralMedio = resistenciaMaxima * 2f / 3f;
        float umbralBajo = resistenciaMaxima / 3f;

        if (EscudoR == null) return;

        if (resistenciaEscudoActual > umbralMedio)
        {
            EscudoR.material = materialEscudoCompleto;
        }
        else if (resistenciaEscudoActual > umbralBajo)
        {
            EscudoR.material = materialEscudoMedio;
        }
        else
        {
            EscudoR.material = materialEscudoBajo;
        }
    }

    // Añadir este método para comprobar periódicamente la regeneración
    private IEnumerator ComprobarRegeneracionEscudo()
    {
        while (true)
        {
            // Si el escudo está activo y se está regenerando
            if (escudoActivo && resistenciaEscudoActual < stats.resistenciaEscudo)
            {
                ActualizarMaterialEscudo();
            }
            
            yield return new WaitForSeconds(0.1f); // Comprobar cada 0.1 segundos
        }
    }
}
