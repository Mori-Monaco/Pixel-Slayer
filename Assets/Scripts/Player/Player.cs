using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase] // чтобы всегда на сцене выбиралс€ Player

public class Player : MonoBehaviour
{

    public static Player instance { get; private set; } // singltone pattern

    [SerializeField] private float movingSpeed = 5f; // SerializeField - видимость в инспекторе
    Vector2 inputVector;

    private Rigidbody2D rb;

    private float minMovingSpeed = 0.1f; // минимальна€ скорость дл€ анимации поко€
    private bool isRunning = false;


    private void Awake() {
        instance = this;
        rb = GetComponent<Rigidbody2D>(); // инициализаци€ RigidBody
    }

    private void Start() {
        GameInput.instance.OnPlayerAttack += Player_OnPlayerAttack;
    }

    private void Update() {
        inputVector = GameInput.instance.GetMovementVector(); // получение вектора движени€

    }

    private void FixedUpdate() {
        HandleMovement();
    }


    private void Player_OnPlayerAttack(object sender, EventArgs e) // функци€ при нажатии Ћ ћ
    {
        Sword.Instance.Attack();
    }





    private void HandleMovement() {
        // inputVector = inputVector.normalized; // делает вектор по диагонали = 1 (с системой InputActions не требуетс€)
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime)); // Time.fixedDeltaTime дл€ плавного движени€

        if (Math.Abs(inputVector.x) > minMovingSpeed || Math.Abs(inputVector.y) > minMovingSpeed) // сравнение по модулю
        {
            isRunning = true;
        }
        else {
            isRunning = false;
        }
    }

    public bool IsRunning() {
        return isRunning;
    }

    public Vector3 GetPlayerScreenPosition() {
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }

}
