using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Enemigo/Stats")]
public class EnemigoStats : ScriptableObject
{
    #region Iniciles

    [Header("Estad�sticas Iniciales")]
    public float vida;
    public float velocidadMovimiento;
    public float velocidadAtaque;
    public float da�o;
    #endregion

    #region Items
    [Header("Bonificaciones por �tems")]
    public float Da�oAdicional;
    public float VelocidadAdicional;
    public float VidaAdicional;
    public float VelocidadAtaqueAdicional;
    #endregion

}