using System.Collections;
using UnityEngine;

public class EnemigoDistancia : Enemigo
{
    public AttackHandler attackHandler; // Añadir referencia a AttackHandler
    public bool atacando = false; // Variable para evitar múltiples corrutinas
    public GameObject objetoAActivar; // Asigna este objeto desde el inspector
    public Camera camara; // Referencia a la cámara

    protected override void Start()
    {
        base.Start();
        attackHandler = GetComponent<AttackHandler>(); // Obtener el componente AttackHandler
    }

    protected override void Update()
    {
        base.Update();

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.transform.position);

        if (distanciaAlJugador <= attackRadius)
        {
            persiguiendoJugador = false;
            velocidadMovimientoActual = 0f;
            if (!isInteractingWithAderezo)
            {
                LanzarProyectilConEfecto();
            }
        }
        else if (distanciaAlJugador <= detectionRadius)
        {
            persiguiendoJugador = true;
            velocidadMovimientoActual = statsEnemigo.velocidadMovimiento;
            PerseguirJugador();
        }
        else
        {
            persiguiendoJugador = false;
            velocidadMovimientoActual = 0f;
        }

        // Hacer que el objeto siempre mire a la cámara
        if (objetoAActivar != null && objetoAActivar.activeSelf && camara != null)
        {
            objetoAActivar.transform.LookAt(
                objetoAActivar.transform.position + camara.transform.rotation * Vector3.forward,
                camara.transform.rotation * Vector3.up
            );
        }
    }

    //public override void Atacck()
    //{

    //    Debug.Log("ejecuto ataque");
    //}

    public override IEnumerator DJugador()
    {
        isDamaging = true;
        while (jugador != null)
        {
            //// Ejecutar el ataque
            //Atacck();
            //Debug.Log("Jugador ha recibido daño");

            //// Activar el sistema de partículas
            //if (damageParticleSystem != null)
            //{
            //    damageParticleSystem.Play();
            //}

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
                velocidadMovimientoInicial = statsEnemigo.velocidadMovimiento;
                PerseguirJugador();
                break;
            }
        }
        isDamaging = false;
        atacando = false; // Restablecer la variable cuando la corrutina termine
    }

    public void LanzarProyectilConEfecto()
    {
        StartCoroutine(ActivarObjetoYLanzarProyectil());
    }

    private IEnumerator ActivarObjetoYLanzarProyectil()
    {
        if (objetoAActivar != null)
            objetoAActivar.SetActive(true);

        // Lanza el proyectil justo después de activar el objeto
        if (attackHandler != null)
            attackHandler.ActivarAtaque();

        // Espera 1 segundo antes de desactivar el objeto
        yield return new WaitForSeconds(2f);

        if (objetoAActivar != null)
            objetoAActivar.SetActive(false);
    }
}




