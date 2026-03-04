using UnityEngine;
using UnityEngine.AI;
using Pixel_Slayer;
using System;


public class EnemyAI : MonoBehaviour
{

    [SerializeField] private State startingState;           // выбор состояния
    [SerializeField] private float roamingDistanceMax = 7f; // макс. расстояние брожения
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;    // время в течение кот. будет бродить

    [SerializeField] private bool isChasingEnemy = true; // преследующий ли враг
    [SerializeField] private float chasingSpeedMultiplier = 2f;
    [SerializeField] private float chasingDistance = 4f;

    [SerializeField] private bool isAttackingEnemy = true; // атакующий ли враг
    [SerializeField] private float attackCooldown = 1f; // задержка
    [SerializeField] private float attackingDistance = 2f; // дистанция атаки

    public event EventHandler OnEnemyAttack;

    private float _nextAttackTime = 0f;

    private NavMeshAgent _navMeshAgent;
    private Vector3 _startingPosition;
    private Vector3 _roamPosition;
    private State _currentState;

    private float _roamingTimer;
    private float _roamingSpeed;
    private float _chasingSpeed;

    private Vector3 _lastPosition;
    private float _nextCheckDirectionTime = 0f; // время след. проверки направления
    private float _CheckDirectionDuration = 0.1f; // шаг между проверками


    public bool IsRunning => _navMeshAgent.velocity != Vector3.zero; // бежит ли враг

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
        _currentState = startingState;

        _chasingSpeed = _navMeshAgent.speed * chasingSpeedMultiplier;
        _roamingSpeed = _navMeshAgent.speed;
    }

    private void Update()
    {
        StateHandler(); // проверка текущего состояния
        MovementDirectionHandler(); // проверка направления
    }

    public void SetDeathState()
    {
        _navMeshAgent.ResetPath(); // сбросить цель
        _currentState = State.Death;
    }

    public float GetRoamingAnimationSpeed()
    {
        return _navMeshAgent.speed / _roamingSpeed;
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
                    _roamingTimer = roamingTimerMax; // обнуляем таймер
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

        if (isChasingEnemy) // если враг преследующий
        {      
            if (distanceToPlayer <= chasingDistance) // если расстояние достаточно маленькое
                newState = State.Chasing;
        }

        if (isAttackingEnemy)
        {
            if (distanceToPlayer <= attackingDistance)
                newState = Player.Instance.IsAlive() ? State.Attacking : State.Roaming; // если игрок жив то атаковать
        }

        if (newState != _currentState)
        {
            if (newState == State.Chasing) // если враг перешел в преследование
            { 
                _navMeshAgent.ResetPath(); // сброс его прошлой цели
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

            _nextAttackTime = Time.time + attackCooldown; // устанавливаем задержку 2 сек.
        }

    }

    private void MovementDirectionHandler()
    {
        if (Time.time > _nextCheckDirectionTime) // если время проверки наступило
        { 
            if (IsRunning) // если враг идет
            {
                ChangeFacingDirection(_lastPosition, transform.position); // поворот в сторону движения
            }
            else if (_currentState == State.Attacking) // если атакует
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
        return _startingPosition + SlayerUtils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax); // получаем новую точку назначения
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition) // поворот в сторону движения
    {
        transform.rotation = sourcePosition.x > targetPosition.x ? Quaternion.Euler(0, -180, 0) : Quaternion.Euler(0, 0, 0);
    }

}
