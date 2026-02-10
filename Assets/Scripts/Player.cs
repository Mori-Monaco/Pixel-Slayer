using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float movingSpeed = 10f; // SerializeField - видимость в инспекторе




    private Rigidbody2D rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // инициализация RigidBody
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = GameInput.instance.GetMovementVector();
        inputVector = inputVector.normalized; // делает вектор по диагонали = 1
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime)); // Time.fixedDeltaTime для плавного движения
    }

}
