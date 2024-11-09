using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerController
{
    private static GameInputs playerInput;

    static PlayerController()
    {
        playerInput = new GameInputs();
        playerInput.Enable();
    }

    public static Vector3 GetMoveInput()
    {
        return playerInput.Player.Move.ReadValue<Vector3>();
    }

    public static bool IsAttackPressed()
    {
        return playerInput.Player.Attack.triggered;
    }

    public static bool Interaccion()
    {
        return playerInput.Player.Interact.triggered;
    }
    public static bool Shield()
    {
        return playerInput.Player.Shield.IsPressed();
    }
}
