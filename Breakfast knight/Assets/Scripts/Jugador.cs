using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Jugador : MonoBehaviour
{
    public Stats stats;

    public abstract void Mover(Vector3 direccion);
    public abstract void ActivarAccion();
    public abstract void ActivarInteraction();
}
