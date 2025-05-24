using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Enemigo/Stats")]
public class EnemigoStats : ScriptableObject
{
    #region Iniciles

    [Header("Estadísticas Iniciales")]
    public float vida;
    public float velocidadMovimiento;
    public float velocidadAtaque;
    public float daño;
    #endregion

    #region Items
    [Header("Bonificaciones por Ítems")]
    public float DañoAdicional;
    public float VelocidadAdicional;
    public float VidaAdicional;
    public float VelocidadAtaqueAdicional;
    #endregion

}