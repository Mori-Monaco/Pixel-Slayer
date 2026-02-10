using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput instance {  get; private set; } // singletone pattern

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }
}
