using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase] // чтобы всегда на сцене выбирался Player

public class Player : MonoBehaviour
{
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnFlashBlink;


    public static Player Instance { get; private set; } // singltone pattern

    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float damageRecoveryTime = 0.5f;

    [Header("Dash Settings")]
    [SerializeField] private int dashSpeed = 4;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCoolDownTime = 0.25f;
    [SerializeField] private TrailRenderer trailRenderer;


    Vector2 inputVector;

    private Rigidbody2D _rb;
    private KnockBack _knockBack;

    private readonly float _minMovingSpeed = 0.1f; // минимальная скорость для анимации покоя
    private bool _isRunning = false;

    private int _currentHealth;
    private bool _canTakeDamage;
    private bool _isAlive;
    private float _initialMovingSpeed;
    private bool _isDashing;

    public bool IsAlive() => _isAlive;

    private Camera _mainCamera;


    private void Awake()
    {
        _initialMovingSpeed = movingSpeed;
        Instance = this;
        _rb = GetComponent<Rigidbody2D>(); // инициализация RigidBody
        _knockBack = GetComponent<KnockBack>();
        _mainCamera = Camera.main; // кеширование объекта MainCamera
    }

    private void Start()
    {
        _canTakeDamage = true;
        _currentHealth = maxHealth;
        GameInput.Instance.OnPlayerAttack += Player_OnPlayerAttack;
        GameInput.Instance.OnPlayerDash += Player_OnPlayerDash;
        _isAlive = true;
    }

    private void Update()
    {
        inputVector = GameInput.Instance.GetMovementVector(); // получение вектора движения

    }

    private void FixedUpdate()
    {
        if (_knockBack.IsGettingKnockBack)
            return;

        HandleMovement();
    }


    public void TakeDamage(Transform damageSource, int damage)
    {
        if (_canTakeDamage && _isAlive)
        {
            _canTakeDamage = false;
            _currentHealth = Math.Max(0, _currentHealth -= damage);
            Debug.Log(_currentHealth);
            _knockBack.GetKnockBack(damageSource);

            OnFlashBlink?.Invoke(this, EventArgs.Empty);

            StartCoroutine(DamageRecoveryRoutine());
        }

        DetectDeath();
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition = _mainCamera.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }

    private void DetectDeath()
    {
        if (_currentHealth == 0 && _isAlive)
        {
            _canTakeDamage = false;
            _isAlive = false;
            _knockBack.StopKnockBackMovement();
            GameInput.Instance.DisabledMovement();

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private IEnumerator DamageRecoveryRoutine() // задержка при получении урона
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        _canTakeDamage = true;
    }


    private void Player_OnPlayerDash(object sender, System.EventArgs e)
    {
        Dash();
    }

    private void Dash() // логика рывка
    {
        if (!_isDashing)
            StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        _isDashing = true;

        movingSpeed *= dashSpeed; // увеличиваем скорость движения
        trailRenderer.emitting = true; // включаем trail эффект
        yield return new WaitForSeconds(dashTime);

        trailRenderer.emitting = false;
        movingSpeed = _initialMovingSpeed;

        yield return new WaitForSeconds(dashCoolDownTime);
        _isDashing = false;
    }


    private void Player_OnPlayerAttack(object sender, EventArgs e) // функция при нажатии ЛКМ
    {
        Sword.Instance.Attack();
    }

    private void HandleMovement()
    {
        _rb.MovePosition(_rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime)); // Time.fixedDeltaTime для плавного движения

        if (Math.Abs(inputVector.x) > _minMovingSpeed || Math.Abs(inputVector.y) > _minMovingSpeed) // сравнение по модулю
            _isRunning = true;
        else
            _isRunning = false;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerDash -= Player_OnPlayerDash;
        GameInput.Instance.OnPlayerAttack -= Player_OnPlayerAttack;
    }

}
