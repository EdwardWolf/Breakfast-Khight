using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderHijo : MonoBehaviour
{
    private Jugador jugador;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject hitParticlesPrefab;

    private GameObject hitParticlesInstance;
    private ParticleSystem hitParticleSystem;

    private Transform enemigoActualTransform;

    private void Start()
    {
        jugador = GetComponentInParent<Jugador>();

        // Instanciar el objeto de partículas una sola vez y desactivarlo
        if (hitParticlesPrefab != null)
        {
            hitParticlesInstance = Instantiate(hitParticlesPrefab, Vector3.zero, Quaternion.identity);
            hitParticlesInstance.SetActive(false);
            hitParticleSystem = hitParticlesInstance.GetComponent<ParticleSystem>();
        }

        GetComponent<Collider>().enabled = false;
    }

   
   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo"))
        {
            Enemigo enemigo = other.GetComponent<Enemigo>();
            if (enemigo != null)
            {
                Debug.Log("Golpeado enemigo");
                // Obtener el arma actual y su daño
                Arma armaActual = jugador.armas[jugador.armaActual];
                float daño = armaActual != null ? armaActual.daño : 0f;
                enemigo.RecibirDanio(daño);

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitDirection = (hitPoint - transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(hitDirection);

                if (hitParticlesInstance != null)
                {
                    if (enemigoActualTransform != other.transform)
                    {
                        enemigoActualTransform = other.transform;
                        hitParticlesInstance.transform.SetParent(enemigoActualTransform);
                    }

                    hitParticlesInstance.transform.position = hitPoint;
                    hitParticlesInstance.transform.rotation = rot;
                    hitParticlesInstance.SetActive(true);

                    if (hitParticleSystem != null)
                    {
                        hitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                        hitParticleSystem.Play();
                    }
                }
            }
        }
    }
}