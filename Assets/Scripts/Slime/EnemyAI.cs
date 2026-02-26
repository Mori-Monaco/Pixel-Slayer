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
        // Idle, пока нет состояния покоя
        Roaming
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
            //case State.Idle: пока нет состояния покоя
            //    break;
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
        startingPosition = transform.position; // обновляем позицию
        roamPosition = GetRoamingPosition(); // ищем новую точку
        ChangeFacingDirection(startingPosition, roamPosition); // поворачиваем врага к цели
        navMeshAgent.SetDestination(roamPosition); // отправляем агента к этой точке
    }

    private Vector3 GetRoamingPosition()
    {
        return startingPosition + SlayerUtils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax); // получаем новую точку назначения
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition) // поворот в сторону движения
    {
        if (sourcePosition.x > targetPosition.x) // если положение правее чем цель
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
