using System.Collections;
using UnityEngine;

public class EnemigoDistancia : Enemigo
{
    public AttackHandler attackHandler; // A�adir referencia a AttackHandler
    private bool atacando = false; // Variable para evitar m�ltiples corrutinas

    protected override void Start()
    {
        base.Start();
        attackHandler = GetComponent<AttackHandler>(); // Obtener el componente AttackHandler
        if (attackHandler == null)
        {
            Debug.LogError("No se encontr� el componente AttackHandler en el objeto.");
        }
    }

    protected override void Update()
    {
        base.Update();
        if (IsPlayerInAttackRange())
        {
            atacando = true;
            StartCoroutine(DJugador());
            
        }
    }

    public override void Atacck()
    {
        attackHandler.ActivarAtaque();
    }

    public override IEnumerator DJugador()
    {
        isDamaging = true;
        while (jugador != null)
        {
            // Ejecutar el ataque
            Atacck();
            Debug.Log("Jugador ha recibido da�o");

            // Activar el sistema de part�culas
            if (damageParticleSystem != null)
            {
                damageParticleSystem.Play();
            }

            // Esperar el cooldown del ataque
            yield return new WaitForSeconds(atackCooldown);

            // Desactivar el sistema de part�culas despu�s de un breve tiempo
            if (damageParticleSystem != null)
            {
                damageParticleSystem.Stop();
            }

            // Verificar si el jugador sigue en rango de ataque
            if (!IsPlayerInAttackRange())
            {
                // Si el jugador est� fuera de rango, perseguirlo
                velocidadMovimiento = statsEnemigo.velocidadMovimiento;
                PerseguirJugador();
                break;
            }
        }
        isDamaging = false;
        atacando = false; // Restablecer la variable cuando la corrutina termine
    }
}




