using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Arma : MonoBehaviour
{
    public string nombreArma;
    public float daño;
    public float velocidadDeAtaque;
    private TrailRenderer trailRenderer; // Referencia al TrailRenderer

    public abstract void Atacar();

    public virtual void Pasiva() { }
    public virtual void Activa() { }

    public virtual void ActivarTrail()
    {
        if (trailRenderer != null)
        {
            trailRenderer.enabled = true; // Activar el rastro
        }
    }

    public virtual void DesactivarTrail()
    {
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false; // Desactivar el rastro
        }
    }

}



