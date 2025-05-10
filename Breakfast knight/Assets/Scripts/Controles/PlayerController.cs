using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance { get; private set; }
    private static GameInputs playerInput;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("PlayerController instance created");
            playerInput = new GameInputs();
            playerInput.Enable();
        }
        else
        {
            Debug.Log("PlayerController instance already exists, destroying this one.");
            Destroy(gameObject);
        }

        Debug.Log("PlayerController instance started and enables status: " + instance.enabled);
    }

    public static Vector3 GetMoveInput()
    {
        return playerInput.Player.Move.ReadValue<Vector3>();
    }

    public static bool IsAttackPressed()
    {
        return playerInput.Player.Attack.triggered;
    }
    public static bool IsAttackCharge()
    {
        return playerInput.Player.Attack.IsPressed();
    }

    public static bool Interaccion()
    {
        return playerInput.Player.Interact.triggered;
    }
    public static bool Shield()
    {
        return playerInput.Player.Shield.IsPressed();
    }
        public static bool Pausa()
    {
        return playerInput.Player.Pausa.triggered;
    }
}
