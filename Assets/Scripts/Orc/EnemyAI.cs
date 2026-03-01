using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pixel_Slayer;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.EventSystems;
using System;


public class EnemyAI : MonoBehaviour
{

    [SerializeField] private State _startingState;           // выбор состояния
    [SerializeField] private float _roamingDistanceMax = 7f; // макс. расстояние
    [SerializeField] private float _roamingDistanceMin = 3f;
    [SerializeField] private float _roamingTimerMax = 2f;    // время в течение кот. будет двигаться

    [SerializeField] private bool _isChasingEnemy = false; // преследующий ли враг
    [SerializeField] private float _chasingDistance = 4f;
    [SerializeField] private float _chasingSpeedMultiplier = 2f;

    [SerializeField] private bool _isAttackingEnemy = false; // атакующий ли враг
    [SerializeField] private float _attackingDistance = 1.5f;
    [SerializeField] private float _attackCooldown = 1f; // задержка между атаками
    private float _nextAttackTime = 0f;

    private NavMeshAgent _navMeshAgent;
    private State _currentState;
    private float _roamingTimer;
    private Vector3 _roamPosition;
    private Vector3 _startingPosition;

    private float _roamingSpeed;
    private float _chasingSpeed;

    private float _nextCheckDirectionTime = 0f; // время когда будет проверка направления
    private float _CheckDirectionDuration = 0.1f; // как часто будет проверка
    private Vector3 _lastPosition; // положение противника

    public event EventHandler OnEnemyAttack;

    private enum State // состояния
    {
        Idle,
        Roaming,
        Chasing, // преследование
        Attacking,
        Death
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _currentState = _startingState;

        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * _chasingSpeedMultiplier;
    }

    private void Update()
    {
        StateHandler(); // проверка текущего состояния
        MovementDirectionHandler(); // проверка направления
    }

    public void SetDeathState()
    {
        _navMeshAgent.ResetPath();
        _currentState = State.Death;
    }

    public float GetRoamingAnimationSpeed()
    {
        return _navMeshAgent.speed / _roamingSpeed;
    }

    public bool IsRunning()
    {
        if (_navMeshAgent.velocity == Vector3.zero) // если враг бежит
        { 
            return false;
        }
        else
        {
            return true;
        }
    }



    private void StateHandler()
    {
        switch (_currentState)
        {
            case State.Roaming:
                _roamingTimer -= Time.deltaTime;     // уменьшаем время
                if (_roamingTimer < 0)
                {
                    Roaming();                     // назначаем новую точку
                    _roamingTimer = _roamingTimerMax; // обнуляем таймер
                }
                CheckCurrentState();
                break;

            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;

            case State.Attacking:
                AttackingTarget();
                CheckCurrentState();
                break;

            case State.Death:
                break;

            default:
            case State.Idle:
                break;
        }
    }



    private void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position); // рассчет растояния до игрока
        State newState = State.Roaming;

        if (_isChasingEnemy) // если враг преследующий
        {      
            if (distanceToPlayer <= _chasingDistance) // если расстояние достаточно маленькое
            { 
                newState = State.Chasing;
            }
        }

        if (_isAttackingEnemy)
        {
            if (distanceToPlayer <= _attackingDistance)
            {
                newState = State.Attacking;
            }

        }

        if (newState != _currentState)
        {
            if (newState == State.Chasing) // если перешел в преследование
            { 
                _navMeshAgent.ResetPath(); // сброс прошлой цели
                _navMeshAgent.speed = _chasingSpeed;
            }
            else if (newState == State.Roaming)
            {
                _roamingTimer = 0f;
                _navMeshAgent.speed = _roamingSpeed;
            }
            else if (newState == State.Attacking)
            {
                _navMeshAgent.ResetPath();
            }

            _currentState = newState;
        }
    }

    private void ChasingTarget()
    {
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }


    private void AttackingTarget()
    {
        if (Time.time > _nextAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty); // выполняем событие атаки

            _nextAttackTime = Time.time + _attackCooldown; // устанавливаем задержку 2 сек.
        }

    }

    private void MovementDirectionHandler()
    {
        if (Time.time > _nextCheckDirectionTime) // если время проверки наступило
        { 
            if (IsRunning())
            {
                ChangeFacingDirection(_lastPosition, transform.position); // поворот в сторону цели
            }
            else if (_currentState == State.Attacking) // если атака
            { 
                ChangeFacingDirection(transform.position, Player.Instance.transform.position); // повернуться к игроку
            }

            _lastPosition = transform.position;
            _nextCheckDirectionTime = Time.time + _CheckDirectionDuration;
        }
    }

    private void Roaming()
    {
        _startingPosition = transform.position; // обновляем позицию
        _roamPosition = GetRoamingPosition(); // ищем новую точку
        _navMeshAgent.SetDestination(_roamPosition);  // отправляем к этой точке
    }

    private Vector3 GetRoamingPosition()
    {
        return _startingPosition + SlayerUtils.GetRandomDir() * UnityEngine.Random.Range(_roamingDistanceMin, _roamingDistanceMax); // получаем новую точку назначения
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition) // поворот в сторону движения
    {
        if (sourcePosition.x > targetPosition.x) // если враг правее чем цель
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

}
