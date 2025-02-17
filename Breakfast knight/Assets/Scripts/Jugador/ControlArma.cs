using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlArma : MonoBehaviour
{
    public Arma armaActual;

    void Update()
    {
        if (PlayerController.IsAttackPressed())
        {
            armaActual.Atacar();
        }
    }
}
