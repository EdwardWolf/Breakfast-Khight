using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Enemigo/Stats")]
public class EnemigoStats : ScriptableObject
{
    public float vida;
    public float velocidadMovimiento;
    public float velocidadAtaque;
    public float daño;
}
