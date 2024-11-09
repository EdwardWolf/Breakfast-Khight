using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Jugador : MonoBehaviour
{
    [SerializeField] protected Stats stats;
    //public Stats stats;
    protected float vidaActual;
    protected float resistenciaEscudoActual;
    protected float velocidadMovimiento;
    protected float velocidadAtaque;

    protected virtual void Start()
    {
        // Asignar los valores iniciales de las estadísticas.
        vidaActual = stats.vida;
        resistenciaEscudoActual = stats.resistenciaEscudo;
        velocidadMovimiento = stats.velocidadMovimiento;
        velocidadAtaque = stats.velocidadAtaque;
    }

    public abstract void Mover(Vector3 direccion);
    public abstract void ActivarAccion();
    public abstract void ActivarInteraction();
}
