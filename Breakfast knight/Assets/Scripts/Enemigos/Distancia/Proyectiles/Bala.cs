using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
    public AttackHandler attackHandler;
    [SerializeField] public float damage = 10f; // Daño que la bala inflige al jugador
    [SerializeField] public float shieldDamage = 5f; // Daño que la bala inflige al escudo
    [SerializeField] private float tiempoDeVida = 5f; // Tiempo de vida de la bala en segundos
    public AudioSource audioSource; // Referencia al componente AudioSource
    public AudioClip impactoClip; // Clip de audio para el sonido del impacto
    
    public void SetAttackHandler(AttackHandler handler)
    {
        attackHandler = handler;
    }

    private void Awake()
    {
        // Obtener el componente AudioSource de la cámara principal
        Camera camara = Camera.main;
        if (camara != null)
        {
            audioSource = camara.GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        // Obtener el AttackHandler del tercer nivel de padres
        Transform parent = transform.parent;
        if (parent != null)
        {
            attackHandler = parent.GetComponent<AttackHandler>();
        }

        // Iniciar la corrutina para regresar la bala después de un tiempo
        StartCoroutine(RegresarBalaDespuesDeTiempo(tiempoDeVida));
    }
    private void OnEnable()
    {
        // Obtener el AttackHandler del tercer nivel de padres
        Transform parent = transform.parent;
        if (parent != null)
        {
            attackHandler = parent.GetComponent<AttackHandler>();
        }

        // Iniciar la corrutina para regresar la bala después de un tiempo
        StartCoroutine(RegresarBalaDespuesDeTiempo(tiempoDeVida));
    }

    public IEnumerator RegresarBalaDespuesDeTiempo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        if (attackHandler != null && attackHandler.ataqueActual != null)
        {
            attackHandler.ataqueActual.RegresarBala(this.gameObject);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reducir la vida del jugador
            Jugador jugador = other.GetComponent<Jugador>();
            if (jugador != null)
            {
                jugador.ReducirVida(damage);
            }

            // Reproducir el sonido del impacto
            if (audioSource != null && impactoClip != null)
            {
                audioSource.PlayOneShot(impactoClip);
            }

            if (attackHandler != null && attackHandler.ataqueActual != null)
            {
                attackHandler.ataqueActual.RegresarBala(this.gameObject);
            }
        }
        else if (other.CompareTag("Muro"))
        {
            Debug.Log("Pego Muro");
            if (attackHandler != null && attackHandler.ataqueActual != null)
            {
                attackHandler.ataqueActual.RegresarBala(this.gameObject);
            }
        }
        else if (other.CompareTag("Escudo"))
        {
            Debug.Log("Pego Escudo");

            // Reducir la resistencia del escudo
            Jugador jugador = other.GetComponentInParent<Jugador>();
            if (jugador != null)
            {
                jugador.ReducirResistenciaEscudo(shieldDamage);
            }

            if (attackHandler != null && attackHandler.ataqueActual != null)
            {
                attackHandler.ataqueActual.RegresarBala(this.gameObject);
            }
        }
    }
}
