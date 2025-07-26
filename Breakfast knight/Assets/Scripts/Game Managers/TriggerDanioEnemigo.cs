using UnityEngine;

public class TriggerDanioEnemigo : MonoBehaviour
{
    public EnemigoCuerpo enemigoCuerpo;

    private void OnTriggerEnter(Collider other)
    {
        if (enemigoCuerpo == null) return;
        if (!enemigoCuerpo.estaAtacando) return;
        if (!other.CompareTag("Player")) return;

        Jugador jugador = other.GetComponent<Jugador>();
        if (jugador != null)
        {
            float dañoFinal = enemigoCuerpo.damage;
            if (enemigoCuerpo.animator != null &&
                enemigoCuerpo.animator.GetCurrentAnimatorStateInfo(0).IsName("CargarAtaque"))
            {
                dañoFinal *= enemigoCuerpo.multiplicadorDañoCargado;
            }

            if (jugador.escudoActivo)
                jugador.ReducirResistenciaEscudo(dañoFinal);
            else
                jugador.ReducirVida(dañoFinal);
        }
    }
}