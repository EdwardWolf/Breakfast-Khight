using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AderezoAtaque : Aderezo
{
    public float incrementoAtaque = 20f; // Incremento de ataque
    public int cantidadGolpes = 5; // Cantidad de golpes antes de restablecer el ataque


    protected override void IncrementarAtaqueJugador()
    {
        if (jugador != null)
        {
            jugador.IncrementarAtaqueTemporal(incrementoAtaque, cantidadGolpes);
            jugador.uiManager.MostrarIncrementoAtaque(incrementoAtaque, cantidadGolpes); // Actualizar la UI
            gameObject.SetActive(false); // Desactivar el objeto instanciado
            Debug.Log("Ataque del jugador incrementado temporalmente");
        }
    }
    protected override void IncrementarAtaqueEnemigo()
    {
        if (enemigo != null)
        {
            enemigo.IncrementarAtaque(incrementoAtaque);
            gameObject.SetActive(false); // Desactivar el objeto instanciado
            Debug.Log("Subio ataque del enemigo "+ enemigo.statsEnemigo.name);
        }
    }
}
