using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaballeroLigero : Jugador
{

    private void Update()
    {
        Vector3 movimiento = PlayerController.GetMoveInput();
        Mover(movimiento);

        if (PlayerController.IsAttackPressed() )
        {
            ActivarAccion();
        }
        if (PlayerController.Interaccion())
        {
            ActivarInteraction();
        }
    }

    public override void Mover(Vector3 direccion)
    {
        // L�gica de movimiento espec�fica para CaballeroLigero
        transform.Translate(direccion * stats.velocidadMovimiento * Time.deltaTime);
    }

    public override void ActivarAccion()
    {
        // L�gica de acci�n espec�fica (ataque, etc.)
        Debug.Log("Caballero Ligero est� atacando");
    }

    public override void ActivarInteraction()
    {
        // L�gica de acci�n espec�fica (ataque, etc.)
        Debug.Log("Caballero Ligero est� Interactuando");
    }
}
