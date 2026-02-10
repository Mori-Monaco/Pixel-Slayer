using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb; // SerializeField - видимость в инспекторе
    private float movingSpeed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // инициализация RigidBody
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = 1f; // увеличение  Y (вверх)
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1f; // уменьшение X (влево)
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = -1f; // уменьшение Y (вниз)
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = 1f; // увеличение X (вправо)
        }

        inputVector = inputVector.normalized; // делает вектор по диагонали = 1

        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime)); // Time.fixedDeltaTime для плавного движения
    }

}
