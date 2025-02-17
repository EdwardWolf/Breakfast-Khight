
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Arma;

[CreateAssetMenu(fileName = "Stats", menuName = "Aderezo/Stats")]
public class StatsAderezos : ScriptableObject
{
    public float IncrementoAtaque;
    public int GolpesDisponibles;

    public void AplicarIncremento(UIManager uiManager)
    {
        uiManager.MostrarIncrementoAtaque(IncrementoAtaque, GolpesDisponibles);
    }

    public enum TipoArma
    {
        Cuchara,
        Tenedor,
        Cuchillo
    }
}
