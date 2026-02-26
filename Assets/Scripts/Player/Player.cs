using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance {  get; private set; } // singltone pattern

    [SerializeField] private float movingSpeed = 7f; // SerializeField - видимость в инспекторе
    Vector2 inputVector;

    private Rigidbody2D rb;

    private float minMovingSpeed = 0.1f; // минимальная скорость для анимации покоя
    private bool isRunning = false;

    public event EventHandler OnSwordSwing; // событие взмаха мечом


    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>(); // инициализация RigidBody
    }

    private void Start()
    {
        GameInput.instance.OnPlayerAttack += Player_OnPlayerAttack;
    }

    private void Update()
    {
        inputVector = GameInput.instance.GetMovementVector(); // получение вектора движения

    }

    private void FixedUpdate()
    {
        HandleMovement();
    }


    private void Player_OnPlayerAttack(object sender, EventArgs e) // функция при нажатии ЛКМ
    {
        Debug.Log("pressed LMB");
        Attack();
    }

    public void Attack()
    {
        OnSwordSwing?.Invoke(this, EventArgs.Empty);
    }


    private void HandleMovement()
    {
        // inputVector = inputVector.normalized; // делает вектор по диагонали = 1 (с системой InputActions не требуется)
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime)); // Time.fixedDeltaTime для плавного движения

        if (Math.Abs(inputVector.x) > minMovingSpeed || Math.Abs(inputVector.y) > minMovingSpeed) // сравнение по модулю
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }

}
