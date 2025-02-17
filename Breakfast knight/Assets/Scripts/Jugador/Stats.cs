using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Jugador/Stats")]
public class Stats : ScriptableObject
{
    public float vida;
    public float resistenciaEscudo;
    public float velocidadMovimiento;
    public float velocidadAtaque;
    public float ataque; // Añadir una variable para el ataque
}
