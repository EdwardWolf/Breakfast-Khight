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
            float da�oFinal = enemigoCuerpo.damage;
            if (enemigoCuerpo.animator != null &&
                enemigoCuerpo.animator.GetCurrentAnimatorStateInfo(0).IsName("CargarAtaque"))
            {
                da�oFinal *= enemigoCuerpo.multiplicadorDa�oCargado;
            }

            if (jugador.escudoActivo)
                jugador.ReducirResistenciaEscudo(da�oFinal);
            else
                jugador.ReducirVida(da�oFinal);
        }
    }
}