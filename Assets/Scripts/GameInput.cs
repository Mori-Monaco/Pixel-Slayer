using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameInput : MonoBehaviour
{
    public static GameInput instance { get; private set; } // singletone pattern

    private PlayerInputActions _playerInputActions;

    public event EventHandler OnPlayerAttack; // событие

    private void Awake()
    {
        instance = this;
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        _playerInputActions.Combat.Attack.started += PlayerAttack_started; // обработчик к нажатию лкм
    }

    public void DisabledMovement()
    {
        _playerInputActions.Disable();
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }

    private void PlayerAttack_started(InputAction.CallbackContext obj)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty); // вызов события
    }

}
