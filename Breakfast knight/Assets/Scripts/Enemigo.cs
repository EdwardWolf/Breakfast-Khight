using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public GameObject canon;
        public EnemigoStats statsEnemigo;
        public float detectionRadius = 5f;
        public LayerMask playerLayer;
        public AERecto ataqueRecto;

    private void Start()
    {
        ataqueRecto = GetComponent<AERecto>();
        canon.SetActive(false);
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

        else
        {
            NoAtacck();
        }
        }

    void Atacck()
    {
        canon.SetActive(true);
    }
    void NoAtacck()
    {
        canon.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        // Cambiar el color de los Gizmos (opcional)
        Gizmos.color = Color.red;

        // Dibujar una esfera en el punto donde está el enemigo con el radio de detección
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

