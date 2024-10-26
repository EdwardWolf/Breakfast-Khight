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
        // Lógica de movimiento específica para CaballeroLigero
        transform.Translate(direccion * stats.velocidadMovimiento * Time.deltaTime);
    }

    public override void ActivarAccion()
    {
        // Lógica de acción específica (ataque, etc.)
        Debug.Log("Caballero Ligero está atacando");
    }

    public override void ActivarInteraction()
    {
        // Lógica de acción específica (ataque, etc.)
        Debug.Log("Caballero Ligero está Interactuando");
    }
}
