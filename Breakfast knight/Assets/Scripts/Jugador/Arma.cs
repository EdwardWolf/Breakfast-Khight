using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Arma : MonoBehaviour
{
        // Atributos del arma.
    public string nombreArma;
public float daño;
public float velocidadDeAtaque;

public abstract void Atacar();
// Método que se encarga de realizar el ataque del arma.


public virtual void Pasiva()
{
}

public virtual void Activa()
{
}
    }



