using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemigo : MonoBehaviour
{
    public EnemigoStats statsEnemigo;
    public float detectionRadius = 5f;
    public float attackRadius = 2f;
    public LayerMask playerLayer;
    public AttackHandler attackHandler; // Añadir referencia a AttackHandler
    private Transform playerTransform; // Añadir referencia al transform del jugador
    public float vidaE; // Vida del enemigo
    public float damage; // Daño del enemigo
    public float velocidadMovimiento; // Velocidad de movimiento del enemigo
    public bool usarRadioDeAtaque = true; // Booleano para activar o desactivar el radio de ataque
    public Image barraDeVida; // Referencia a la imagen de la barra de vida
    private Camera camara; // Añadir referencia a la cámara
    public GameObject aderezoPrefab; // Prefab del objeto Aderezo
    public float dropChanceMin = 0.1f; // Probabilidad mínima de dropear el objeto
    public float dropChanceMax = 0.5f; // Probabilidad máxima de dropear el objeto
    public Transform dropPosition; // Referencia al objeto hijo donde se dropeará el Aderezo
    private Jugador jugador;
    private bool isDamaging = false;
    public GameObject attackEffect; // Efecto que se ejecutará antes del ataque
    public ParticleSystem damageParticleSystem; // Sistema de partículas para el daño
    private Renderer renderer; // Referencia al Renderer del enemigo
    public float atackCooldown = 1.5f; // Cooldown del ataque

    private void Start()
    {
        camara = Camera.main; // Obtener la cámara principal    
        attackHandler = GetComponent<AttackHandler>(); // Obtener el componente AttackHandler
        vidaE = statsEnemigo.vida; // Inicializar la vida del enemigo
        ActualizarBarraDeVida(); // Inicializar la barra de vida
        velocidadMovimiento = statsEnemigo.velocidadMovimiento; // Inicializar la velocidad de movimiento
        damage = statsEnemigo.daño; // Inicializar el daño
        renderer = GetComponent<Renderer>(); // Obtener el Renderer del enemigo
    }

    void Update()
    {
        barraDeVida.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
                         camara.transform.rotation * Vector3.up);
        DetectPlayer();
        if (playerTransform != null)
        {
            LookAtPlayer();
            if (usarRadioDeAtaque && IsPlayerInAttackRange())
            {
                Atacck();
            }
            else
            {
                PerseguirJugador();
            }
        }
    }

    void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            playerTransform = hits[0].transform; // Guardar la referencia al transform del jugador
        }
        else
        {
            playerTransform = null; // Si no hay jugador, resetear la referencia
        }
    }

    bool IsPlayerInAttackRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, playerLayer);
        return hits.Length > 0;
    }

    void Atacck()
    {
        StartCoroutine(PreAttackEffect());
    }

    IEnumerator PreAttackEffect()
    {
        // Ejecutar el efecto antes del ataque
        if (attackEffect != null)
        {
            attackEffect.SetActive(true);
        }

        // Esperar un segundo
        yield return new WaitForSeconds(1f);

        // Desactivar el efecto
        if (attackEffect != null)
        {
            attackEffect.SetActive(false);
        }

        // Ejecutar el ataque
        attackHandler.ActivarAtaque();
    }

    void LookAtPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void PerseguirJugador()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * velocidadMovimiento * Time.deltaTime;
    }

    public void RecibirDanio(float cantidad)
    {
        vidaE -= cantidad;
        ActualizarBarraDeVida();
        if (vidaE <= 0)
        {
            DropAderezo();
            DesactivarEnemigo();
        }
    }

    void ActualizarBarraDeVida()
    {
        if (barraDeVida != null)
        {
            barraDeVida.fillAmount = vidaE / statsEnemigo.vida;
        }
    }

    void DesactivarEnemigo()
    {
        // Desactivar el objeto del enemigo
        gameObject.SetActive(false);
        Debug.Log("Enemigo ha sido derrotado");
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
        if (Random.value <= dropChance)
        {
            if (dropPosition != null)
            {
                Instantiate(aderezoPrefab, dropPosition.position, Quaternion.identity);
                Debug.Log("Aderezo dropeado en la posición: " + dropPosition.position);
            }
            else
            {
                Instantiate(aderezoPrefab, transform.position, Quaternion.identity);
                Debug.Log("Aderezo dropeado en la posición: " + transform.position);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemigo ha colisionado con el jugador");
            jugador = collision.gameObject.GetComponent<Jugador>();
            if (jugador != null && !isDamaging)
            {
                StartCoroutine(DañarJugador());
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            jugador = null;
            isDamaging = false;
            StopAllCoroutines();
            velocidadMovimiento = statsEnemigo.velocidadMovimiento; // Detener el movimiento del enemigo
        }
    }

    private IEnumerator DañarJugador()
    {
        isDamaging = true;
        while (jugador != null)
        {
            yield return new WaitForSeconds(atackCooldown);

            if (jugador != null)
            {
                velocidadMovimiento = 0f; // Detener el movimiento del enemigo
                jugador.ReducirVida(damage);
                Debug.Log("Jugador ha recibido daño");

                // Activar el sistema de partículas
                if (damageParticleSystem != null)
                {
                    damageParticleSystem.Play();
                }

                // Desactivar el sistema de partículas después de un breve tiempo
                yield return new WaitForSeconds(0.5f);
                if (damageParticleSystem != null)
                {
                    damageParticleSystem.Stop();
                }
            }
        }
        isDamaging = false;
    }

    public SectionManager sectionManager; // Referencia al SectionManager
}




