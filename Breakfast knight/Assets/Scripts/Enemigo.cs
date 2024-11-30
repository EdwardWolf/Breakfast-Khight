using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public EnemigoStats statsEnemigo;
    public float detectionRadius = 5f;
    public LayerMask playerLayer;
    public AttackHandler attackHandler; // Añadir referencia a AttackHandler

    private void Start()
    {
        attackHandler = GetComponent<AttackHandler>(); // Obtener el componente AttackHandler
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            Atacck();
        }
    }

    void Atacck()
    {
        attackHandler.ActivarAtaque(); // Llamar al método para activar el ataque
    }

    private void OnDrawGizmosSelected()
    {
        // Cambiar el color de los Gizmos (opcional)
        Gizmos.color = Color.red;

        // Dibujar una esfera en el punto donde está el enemigo con el radio de detección
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}


