using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase] // чтобы всегда на сцене выбирался Player

public class Player : MonoBehaviour
{
    public event EventHandler OnPlayerDeath;

    public static Player Instance { get; private set; } // singltone pattern

    [SerializeField] private float _movingSpeed = 5f; // SerializeField - видимость в инспекторе
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private float _damageRecoveryTime = 0.5f;


    Vector2 inputVector;

    private Rigidbody2D _rb;
    private KnockBack _knockBack;

    private float _minMovingSpeed = 0.1f; // минимальная скорость для анимации покоя
    private bool _isRunning = false;

    private int _currentHealth;
    private bool _canTakeDamage;
    private bool _isAlive;


    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>(); // инициализация RigidBody
        _knockBack = GetComponent<KnockBack>();
    }

    private void Start()
    {
        _canTakeDamage = true;
        _currentHealth = _maxHealth;
        GameInput.instance.OnPlayerAttack += Player_OnPlayerAttack;
        _isAlive = true;
    }

    private void Update()
    {
        inputVector = GameInput.instance.GetMovementVector(); // получение вектора движения

    }

    private void FixedUpdate()
    {
        if (_knockBack.IsGettingKnockBack)
            return;

        HandleMovement();
    }

    public bool IsAlive() => _isAlive;

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (_canTakeDamage && _isAlive)
        {
            _canTakeDamage = false;
            _currentHealth = Math.Max(0, _currentHealth -= damage);
            Debug.Log(_currentHealth);
            _knockBack.GetKnockBack(damageSource);

            StartCoroutine(DamageRecoveryRoutine());
        }

        DetectDeath();
    }

    private void DetectDeath()
    {
        if (_currentHealth == 0 && _isAlive)
        {
            _canTakeDamage = false;
            _isAlive = false;
            _knockBack.StopKnockBackMovement();
            GameInput.instance.DisabledMovement();

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private IEnumerator DamageRecoveryRoutine() // задержка при получении урона
    {
        yield return new WaitForSeconds(_damageRecoveryTime);
        _canTakeDamage = true;
    }


    public bool IsRunning()
    {
        return _isRunning;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }


    private void Player_OnPlayerAttack(object sender, EventArgs e) // функция при нажатии ЛКМ
    {
        Sword.Instance.Attack();
    }


    private void HandleMovement()
    {
        // inputVector = inputVector.normalized; // делает вектор по диагонали = 1 (с системой InputActions не требуется)
        _rb.MovePosition(_rb.position + inputVector * (_movingSpeed * Time.fixedDeltaTime)); // Time.fixedDeltaTime для плавного движения

        if (Math.Abs(inputVector.x) > _minMovingSpeed || Math.Abs(inputVector.y) > _minMovingSpeed) // сравнение по модулю
        {
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }
    }

}
