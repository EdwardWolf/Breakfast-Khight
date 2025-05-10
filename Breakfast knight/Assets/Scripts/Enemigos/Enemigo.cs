using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemigo : MonoBehaviour
{
    public EnemigoStats statsEnemigo;
    public float detectionRadius;
    public float attackRadius;
    public LayerMask playerLayer;
    public LayerMask aderezoLayer; // Añadir una capa para los aderezos
    public Transform playerTransform; // Añadir referencia al transform del jugador
    public float vidaE; // Vida del enemigo
    public float damage; // Daño del enemigo
    public float velocidadMovimientoInicial; // Velocidad de movimiento del enemigo
    public float velocidadMovimientoActual; // Velocidad de movimiento del enemigo
    public bool usarRadioDeAtaque = true; // Booleano para activar o desactivar el radio de ataque
    public Image barraDeVida; // Referencia a la imagen de la barra de vida
    private Camera camara; // Añadir referencia a la cámara
    public List<GameObject> aderezosPrefabs; // Lista de prefabs de aderezos
    public float dropChanceMin = 0.1f; // Probabilidad mínima de dropear el objeto
    public float dropChanceMax = 0.5f; // Probabilidad máxima de dropear el objeto
    public Transform dropPosition; // Referencia al objeto hijo donde se dropeará el Aderezo
    public Jugador jugador;
    public bool isDamaging = false;
    public ParticleSystem damageParticleSystem; // Sistema de partículas para el daño
    public float atackCooldown = 1.5f; // Cooldown del ataque
    public float tiempoParaSoltarObjeto = 5f; // Tiempo que debe pasar antes de soltar el objeto
    public bool persiguiendoJugador = false; // Indica si el enemigo está persiguiendo al jugador
    public bool puedeSoltarObjeto = true; // Booleano para activar o desactivar la funcionalidad de soltar el objeto
    public Animator animator; // Referencia al componente Animator
    public AudioSource audioSource; // Referencia al componente AudioSource
    public AudioClip golpeClip; // Clip de audio para el sonido del golpe

    // Nueva variable para la probabilidad de uso del aderezo
    [Range(1, 10)]
    public float probabilidadUsoAderezo = 3f; // Probabilidad de uso del aderezo (1-10)

    // Nueva variable para almacenar la referencia al aderezo detectado
    private Transform aderezoTransform;
    public Image interactionProgressBar;

    private bool isInteractingWithAderezo = false;
    private float interactionTimer = 0f;
    public float interactionTime = 2f; // Tiempo necesario para completar la interacción

    [Range(0, 1)]
    public float probabilidadAlejarse = 0.3f; // Probabilidad de alejarse (0-1)
    public float distanciaAlejarse = 5f; // Distancia a la que se alejará el enemigo
    public bool puedeAlejarse = true; // Booleano para activar o desactivar la opción de alejarse

    private Coroutine dañoCoroutine;
    public Coroutine reducirResistenciaEscudoCoroutine;

    public float fuerzaEmpuje = 5f; // Fuerza de empuje al jugador

    private EnemySpawner spawner;

    public Collider escudoCollider;
    protected virtual void Awake()
    {
        camara = Camera.main; // Obtener la cámara principal
        if (camara != null)
        {
            audioSource = camara.GetComponent<AudioSource>(); // Obtener el componente AudioSource de la cámara
        }
    }

    protected virtual void Start()
    {
        vidaE = statsEnemigo.vida; // Inicializar la vida del enemigo
        ActualizarBarraDeVida(); // Inicializar la barra de vida
        velocidadMovimientoInicial = statsEnemigo.velocidadMovimiento; // Inicializar la velocidad de movimiento
        velocidadMovimientoActual = velocidadMovimientoInicial; // Inicializar la velocidad de movimiento actual
        damage = statsEnemigo.daño; // Inicializar el daño
        animator = GetComponent<Animator>(); // Obtener el componente Animator
        spawner = GetComponentInParent<EnemySpawner>();
    }

    protected virtual void Update()
    {
        barraDeVida.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
                         camara.transform.rotation * Vector3.up);
        DetectPlayer();
        if (aderezoTransform != null)
        {
            if (aderezoTransform.gameObject.activeSelf)
            {
                PerseguirAderezo();
            }
            else
            {
                aderezoTransform = null; // Resetear la referencia si el aderezo ya no está activo
            }
        }
        else if (playerTransform != null)
        {
            LookAtPlayer();
            if (usarRadioDeAtaque && IsPlayerInAttackRange())
            {
                if (!isDamaging)
                {
                    dañoCoroutine = StartCoroutine(DJugador());
                }
            }
            else
            {
                if (!persiguiendoJugador)
                {
                    persiguiendoJugador = true;
                }
                PerseguirJugador();
            }
        }
        else
        {
            persiguiendoJugador = false;
        }

        // Verificar si el collider del escudo está desactivado y limpiar la referencia
        if (escudoCollider != null && !escudoCollider.enabled)
        {
            enContactoConEscudo = false;
            escudoCollider = null;
            if (reducirResistenciaEscudoCoroutine != null)
            {
                StopCoroutine(reducirResistenciaEscudoCoroutine);
                reducirResistenciaEscudoCoroutine = null;
            }
        }
    }


    public void IncrementarAtaque(float cantidad)
    {
        damage += cantidad; // Incrementar el daño del enemigo
    }

    protected virtual void DetectPlayer()
    {
        Collider[] playerHits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (playerHits.Length > 0)
        {
            playerTransform = playerHits[0].transform; // Guardar la referencia al transform del jugador
            jugador = playerHits[0].GetComponent<Jugador>(); // Guardar la referencia al componente Jugador
            if (animator != null)
            {
                animator.SetTrigger("DetectarJugador"); // Activar el Trigger de la animación de detección
            }
        }
        else
        {
            playerTransform = null; // Si no hay jugador, resetear la referencia
            jugador = null; // Resetear la referencia al componente Jugador
        }

        // Solo detectar aderezos si no se tiene uno actualmente
        if (aderezoTransform == null)
        {
            Collider[] aderezoHits = Physics.OverlapSphere(transform.position, detectionRadius, aderezoLayer);
            if (aderezoHits.Length > 0)
            {
                // Verificar la probabilidad de ir por el aderezo
                if (Random.Range(1, 11) <= probabilidadUsoAderezo)
                {
                    aderezoTransform = aderezoHits[0].transform; // Guardar la referencia al transform del aderezo
                }
                else
                {
                    aderezoTransform = null; // Si no se cumple la probabilidad, resetear la referencia
                }
            }
            else
            {
                aderezoTransform = null; // Si no hay aderezo, resetear la referencia
            }
        }
    }


    public bool IsPlayerInAttackRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, playerLayer);
        return hits.Length > 0;
    }

    public virtual void Atacck()
    {
        if (jugador != null)
        {
            jugador.ReducirVida(damage); // Asegurarse de que el jugador reciba daño
            if (audioSource != null && golpeClip != null)
            {
                audioSource.PlayOneShot(golpeClip); // Reproducir el sonido del golpe
            }
        }
    }
    public virtual IEnumerator DJugador()
    {
        isDamaging = true;
        while (jugador != null)
        {
            // Ejecutar el ataque
            Atacck();
            Debug.Log("Jugador ha recibido daño");

            // Activar el sistema de partículas
            if (damageParticleSystem != null)
            {
                damageParticleSystem.Play();
            }

            // Eliminar la lógica de empuje al jugador
            /*
            if (jugador != null)
            {
                Vector3 direccionEmpuje = (jugador.transform.position - transform.position).normalized;
                Rigidbody rbJugador = jugador.GetComponent<Rigidbody>();
                if (rbJugador != null)
                {
                    rbJugador.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode.Impulse);
                }
            }
            */

            // Esperar el cooldown del ataque
            yield return new WaitForSeconds(atackCooldown);

            // Desactivar el sistema de partículas después de un breve tiempo
            if (damageParticleSystem != null)
            {
                damageParticleSystem.Stop();
            }

            // Verificar si el jugador sigue en rango de ataque
            if (!IsPlayerInAttackRange())
            {
                // Si el jugador está fuera de rango, perseguirlo
                velocidadMovimientoActual = velocidadMovimientoInicial;
                PerseguirJugador();
                break;
            }
        }
        isDamaging = false;
    }

    void LookAtPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    void LookAtAderezo()
    {
        Vector3 direction = (aderezoTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    public virtual void PerseguirJugador()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * velocidadMovimientoActual * Time.deltaTime;
        }

    }

    public void PerseguirAderezo()
    {
        if (aderezoTransform == null)
        {
            // Si el aderezo ha sido destruido, dejar de perseguirlo
            isInteractingWithAderezo = false;
            interactionTimer = 0f;
            UpdateProgressBar(0f);
            return;
        }

        LookAtAderezo();

        if (!isInteractingWithAderezo)
        {
            Vector3 direction = (aderezoTransform.position - transform.position).normalized;
            transform.position += direction * velocidadMovimientoActual * Time.deltaTime;

            if (Vector3.Distance(transform.position, aderezoTransform.position) < 1f) // Ajusta la distancia según sea necesario
            {
                isInteractingWithAderezo = true;
                interactionTimer = 0f;

                // Obtener la referencia a la barra de interacción del aderezo
                Aderezo aderezo = aderezoTransform.GetComponent<Aderezo>();
                if (aderezo != null)
                {
                    interactionProgressBar = aderezo.interactionProgressBar;
                    interactionProgressBar.gameObject.SetActive(true); // Asegurarse de que la barra de progreso esté visible
                }
            }
        }
        else
        {
            interactionTimer += Time.deltaTime;
            UpdateProgressBar(interactionTimer / interactionTime);

            if (interactionTimer >= interactionTime)
            {
                Destroy(aderezoTransform.gameObject);
                aderezoTransform = null;
                isInteractingWithAderezo = false;
                interactionTimer = 0f;
                UpdateProgressBar(0f);
                if (interactionProgressBar != null)
                {
                    interactionProgressBar.gameObject.SetActive(false); // Ocultar la barra de progreso
                }
            }
        }
    }

    private void UpdateProgressBar(float progress)
    {
        if (interactionProgressBar != null)
        {
            interactionProgressBar.fillAmount = progress;
        }
    }

    public void RecibirDanio(float cantidad)
    {
        // Reducir la vida del enemigo
        vidaE -= cantidad;

        // Mostrar en consola el daño recibido y la vida restante
        Debug.Log($"El enemigo {gameObject.name} recibió {cantidad} de daño. Vida restante: {vidaE}");

        // Actualizar la barra de vida
        ActualizarBarraDeVida();

        // Verificar si el enemigo ha muerto
        if (vidaE <= 0)
        {
            Debug.Log($"El enemigo {gameObject.name} ha sido derrotado.");
            DropAderezo();
            DesactivarEnemigo();
        }
        else
        {
            // Verificar la probabilidad de alejarse
            if (puedeAlejarse && Random.value <= probabilidadAlejarse)
            {
                Alejarse();
            }
        }
    }


    private void Alejarse()
    {
        // Calcular una dirección aleatoria para alejarse
        Vector3 alejarseDireccion = (transform.position - jugador.transform.position).normalized;
        Vector3 nuevaPosicion = transform.position + alejarseDireccion * distanciaAlejarse;

        // Mover al enemigo a la nueva posición
        transform.position = Vector3.Lerp(transform.position, nuevaPosicion, Time.deltaTime * velocidadMovimientoActual);
    }
    public void ActualizarBarraDeVida()
    {
        if (barraDeVida != null)
        {
            barraDeVida.fillAmount = vidaE / statsEnemigo.vida;
        }
    }

    public void DesactivarEnemigo()
    {
        // Desactivar el objeto del enemigo
        gameObject.SetActive(false);
        spawner.RegresarEnemigo(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Cambiar el color de los Gizmos (opcional)
        Gizmos.color = Color.red;
        // Dibujar una esfera en el punto donde está el enemigo con el radio de detección
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (usarRadioDeAtaque)
        {
            Gizmos.color = Color.blue;
            // Dibujar una esfera en el punto donde está el enemigo con el radio de ataque
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }

    private void DropAderezo()
    {
        float dropChance = Random.Range(dropChanceMin, dropChanceMax);
        if (Random.value <= dropChance && aderezosPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, aderezosPrefabs.Count);
            GameObject aderezoToDrop = aderezosPrefabs[randomIndex];
            if (dropPosition != null)
            {
                Instantiate(aderezoToDrop, dropPosition.position, Quaternion.identity);
                Debug.Log("Aderezo dropeado en la posición: " + dropPosition.position);
            }
            else
            {
                Instantiate(aderezoToDrop, transform.position, Quaternion.identity);
                Debug.Log("Aderezo dropeado en la posición: " + transform.position);
            }
        }
    }

    public float shieldDamage = 10f; // Daño al escudo
    public bool enContactoConEscudo = false;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !enContactoConEscudo)
        {
            velocidadMovimientoActual = 0f;
            persiguiendoJugador = false;
            Debug.Log("Enemigo ha colisionado con el jugador");
            jugador = collision.gameObject.GetComponent<Jugador>();
            if (jugador != null && !isDamaging)
            {
                dañoCoroutine = StartCoroutine(EsperarYHacerDanio());
            }
            // Detener el movimiento del enemigo
            velocidadMovimientoActual = 0f;
            // Dejar de soltar el objeto adicional
            puedeSoltarObjeto = false;
        }
        else if (collision.gameObject.CompareTag("Escudo"))
        {
            enContactoConEscudo = true;
            jugador = collision.gameObject.GetComponentInParent<Jugador>();
            escudoCollider = collision.collider; // Obtener la referencia del collider del escudo
            velocidadMovimientoActual = 0f;
            if (jugador != null)
            {
                reducirResistenciaEscudoCoroutine = StartCoroutine(EsperarYReducirResistenciaEscudo());
            }
        }
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemigo ha dejado de colisionar con el jugador");
            jugador = null;
            isDamaging = false;

            if (dañoCoroutine != null)
            {
                StopCoroutine(dañoCoroutine);
                dañoCoroutine = null;
            }
            velocidadMovimientoActual = velocidadMovimientoInicial; // Restaurar la velocidad de movimiento del enemigo
            puedeSoltarObjeto = true;
            if (this is EnemigoCuerpo enemigoCuerpo)
            {
                enemigoCuerpo.AlcanzarJugador();
            }
        }
        else if (collision.gameObject.CompareTag("Escudo"))
        {
            Debug.Log("Enemigo ha dejado de colisionar con el escudo");
            enContactoConEscudo = false;
            escudoCollider = null; // Limpiar la referencia del collider del escudo
            velocidadMovimientoActual = velocidadMovimientoInicial; // Restaurar la velocidad de movimiento del enemigo
            Debug.Log("Se restauro la velocidad");
            if (reducirResistenciaEscudoCoroutine != null)
            {
                StopCoroutine(reducirResistenciaEscudoCoroutine);
                reducirResistenciaEscudoCoroutine = null;
            }

            // Asegurarse de que el enemigo vuelva a perseguir al jugador
            if (playerTransform != null)
            {
                persiguiendoJugador = true;
            }
        }
    }


    private IEnumerator EsperarYHacerDanio()
    {
        yield return new WaitForSeconds(1f); // Esperar un segundo
        if (jugador != null)
        {
            dañoCoroutine = StartCoroutine(DJugador()); // Iniciar la corrutina para hacer daño al jugador
        }
    }

    public IEnumerator EsperarYReducirResistenciaEscudo()
    {
        while (enContactoConEscudo)
        {
            yield return new WaitForSeconds(2f); // Esperar dos segundos
            if (jugador != null && escudoCollider != null)
            {
                jugador.ReducirResistenciaEscudo(shieldDamage); // Reducir la resistencia del escudo
            }
        }
    }


    public SectionManager sectionManager; // Referencia al SectionManager

    private Coroutine ralentizacionCoroutine;
    public void AplicarRalentizacion(float factor, float duracion)
    {
        if (ralentizacionCoroutine != null)
        {
            StopCoroutine(ralentizacionCoroutine);
        }
        ralentizacionCoroutine = StartCoroutine(Ralentizar(factor, duracion));
    }
    private IEnumerator Ralentizar(float factor, float duracion)
    {
        velocidadMovimientoActual *= factor;
        yield return new WaitForSeconds(duracion);
        velocidadMovimientoActual = velocidadMovimientoInicial; // Inicializar la velocidad de movimiento
    }
}
