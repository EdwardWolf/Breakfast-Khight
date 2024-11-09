using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaballeroLigero : Jugador
{
    private bool escudoActivo = false;

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
        if (PlayerController.Shield())
        {
            ActivarEscudo();
        }
        else
        {
            DesactivarEscudo();
        }

    }

    public override void Mover(Vector3 direccion)
    {
        // Lógica de movimiento específica para CaballeroLigero
        transform.Translate(direccion * stats.velocidadMovimiento * Time.deltaTime);
    }

    public override void ActivarAccion()
    {
        // Ataque simple
        Debug.Log("Caballero Ligero está atacando");
    }

    public override void ActivarInteraction()
    {
        // Interactuar
        Debug.Log("Caballero Ligero está Interactuando");
    }
    public void ActivarEscudo()
    {
        if (!escudoActivo)
        {
            //escudo.SetActive(true);  // Activar el objeto escudo en la escena.
            escudoActivo = true;
            Debug.Log("Escudo activado.");
        }
    }

    public void DesactivarEscudo()
    {
        if (escudoActivo)
        {
            //escudo.SetActive(false);  // Desactivar el objeto escudo.
            escudoActivo = false;
            Debug.Log("Escudo desactivado.");
        }
    }
}
