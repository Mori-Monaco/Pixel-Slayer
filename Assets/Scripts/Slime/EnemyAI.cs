using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pixel_Slayer;


public class EnemyAI : MonoBehaviour
{

    [SerializeField] private State startingState; // выбор состояния
    [SerializeField] private float roamingDistanceMax = 7f; // макс. расстояние
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f; // время в течение кот. будет двигаться

    private NavMeshAgent navMeshAgent;
    private State state;
    private float roamingTime;
    private Vector3 roamPosition;
    private Vector3 startingPosition;


    private enum State // состояния
    {
        Idle,
        Roaming
    }

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        state = startingState;
    }

    private void Update() // проверка текущего состояния
    {
        switch (state)
        {
            default:
            case State.Idle:
                break;
            case State.Roaming:
                roamingTime -= Time.deltaTime; // уменьшаем время
                if (roamingTime < 0)
                {
                    Roaming();                     // назначаем новую точку
                    roamingTime = roamingTimerMax; // снова делаем время максимальным
                }
                break;
        }
    }

    private void Roaming()
    {
        roamPosition = GetRoamingPosition(); // ищем новую точку
        navMeshAgent.SetDestination(roamPosition); // отправляем агента к этой точке
    }

    private Vector3 GetRoamingPosition()
    {
        return startingPosition + SlayerUtils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }
}
