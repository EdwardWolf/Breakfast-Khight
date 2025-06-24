using System.Collections;
using UnityEngine;

public class EnemigoDistancia : Enemigo
{
    public AttackHandler attackHandler; // Añadir referencia a AttackHandler
    public bool atacando = false; // Variable para evitar múltiples corrutinas

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
            // Detener movimiento y atacar
            persiguiendoJugador = false;
            velocidadMovimientoActual = 0f;
            if (!isInteractingWithAderezo)
            {
                attackHandler.ActivarAtaque();
            }
        }
        else if (distanciaAlJugador <= detectionRadius)
        {
            // Perseguir al jugador
            persiguiendoJugador = true;
            velocidadMovimientoActual = statsEnemigo.velocidadMovimiento;
            PerseguirJugador();
        }
        else
        {
            // Fuera de ambos rangos, detenerse
            persiguiendoJugador = false;
            velocidadMovimientoActual = 0f;
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
}




