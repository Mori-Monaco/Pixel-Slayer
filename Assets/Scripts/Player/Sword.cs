using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

    [SerializeField] private int damageAmount = 2; // наносимый урон врагу
    public static Sword Instance { get; private set; }

    private PolygonCollider2D _polygonCollider2D;
    public event EventHandler OnSwordSwing; // событие взмаха мечом

    private void Awake()
    {
        Instance = this;
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        AttackColliderTurnOff(); // при старте область удара выключена
    }

    private void Update()
    {
        FollowMousePosition();
    }

    public void Attack()
    {
        _polygonCollider2D.enabled = true;  // включаем коллайдер
        Invoke(nameof(TurnOffCollider), 0.1f);  // выключаем через 100 мс
        OnSwordSwing?.Invoke(this, EventArgs.Empty);
    }

    private void TurnOffCollider()
    {
        _polygonCollider2D.enabled = false;  // выключаем коллайдер
    }

    public void AttackColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    { // коллизия меча и врага
        if (collision.transform.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
        }
    }

    private void FollowMousePosition()
    {
        Vector3 mousePos = GameInput.Instance.GetMousePosition();           // получаем позицию курсора
        Vector3 playerPosition = Player.Instance.GetPlayerScreenPosition(); // получаем позицию игрока

        if (mousePos.x < playerPosition.x)
        {                  // если курсор левее игрока
            transform.rotation = Quaternion.Euler(0, 180, 0); // поворачиваем область удара влево
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    //private void AttackColliderTurnOn()
    //{
    //    _polygonCollider2D.enabled = true;
    //}

    //private void AttackColliderTurnOffOn()
    //{
    //    AttackColliderTurnOff();
    //    AttackColliderTurnOn();
    //}
}
