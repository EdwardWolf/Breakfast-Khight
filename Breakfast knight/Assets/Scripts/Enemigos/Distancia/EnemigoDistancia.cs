using System.Collections;
using UnityEngine;

public class EnemigoDistancia : Enemigo
{
    public AttackHandler attackHandler; // Añadir referencia a AttackHandler
    private bool atacando = false; // Variable para evitar múltiples corrutinas

    protected override void Start()
    {
        base.Start();
        attackHandler = GetComponent<AttackHandler>(); // Obtener el componente AttackHandler
        if (attackHandler == null)
        {
            Debug.LogError("No se encontró el componente AttackHandler en el objeto.");
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
            Debug.Log("Jugador ha recibido daño");

            // Activar el sistema de partículas
            if (damageParticleSystem != null)
            {
                damageParticleSystem.Play();
            }

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
                velocidadMovimiento = statsEnemigo.velocidadMovimiento;
                PerseguirJugador();
                break;
            }
        }
        isDamaging = false;
        atacando = false; // Restablecer la variable cuando la corrutina termine
    }
}




