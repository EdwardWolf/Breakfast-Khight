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
    public float vidaE = 100f;
    public float velocidadMovimiento = 2f; // Velocidad de movimiento del enemigo
    public bool usarRadioDeAtaque = true; // Booleano para activar o desactivar el radio de ataque
    public Image barraDeVida; // Referencia a la imagen de la barra de vida

    private void Start()
    {
        attackHandler = GetComponent<AttackHandler>(); // Obtener el componente AttackHandler
        vidaE = statsEnemigo.vida; // Inicializar la vida del enemigo
        ActualizarBarraDeVida(); // Inicializar la barra de vida
    }

    void Update()
    {
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
        attackHandler.ActivarAtaque(); // Llamar al método para activar el ataque
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
}
