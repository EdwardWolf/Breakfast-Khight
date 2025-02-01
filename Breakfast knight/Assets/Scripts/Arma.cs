using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Arma : MonoBehaviour
{

    // Atributos del arma.
    public string nombreArma;
    public float daño;
    public float alcance;

    public abstract void Atacar();

    public virtual void Pasiva()
    {
    }

    public virtual void Activa()
    {
    } 


}
