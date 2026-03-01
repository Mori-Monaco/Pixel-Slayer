using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    [SerializeField] private int _damageAmount = 2; // наносимый урон врагу
    public static Sword Instance { get; private set; }

    private PolygonCollider2D _polygonCollider2D;
    public event EventHandler OnSwordSwing; // событие взмаха мечом

    private void Awake() {
        Instance = this;
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Start() {
        AttackColliderTurnOff(); // при старте область удара выключена
    }

    private void Update() {
        FollowMousePosition();
    }

    public void Attack() {
        AttackColliderTurnOffOn(); // вкл/выкл области атаки
        OnSwordSwing?.Invoke(this, EventArgs.Empty);
    }

    private void OnTriggerEnter2D(Collider2D collision) { // коллизия меча и врага
        if (collision.transform.TryGetComponent(out EnemyEntity enemyEntity)) {
            enemyEntity.TakeDamage(_damageAmount);
        }
    }


    private void FollowMousePosition() {
        Vector3 mousePos = GameInput.instance.GetMousePosition();           // получаем позицию курсора
        Vector3 playerPosition = Player.instance.GetPlayerScreenPosition(); // получаем позицию игрока

        if (mousePos.x < playerPosition.x) {                  // если курсор левее игрока
            transform.rotation = Quaternion.Euler(0, 180, 0); // поворачиваем область удара влево
        }
        else {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void AttackColliderTurnOff() {
        _polygonCollider2D.enabled = false;
    }

    private void AttackColliderTurnOn() {
        _polygonCollider2D.enabled = true;
    }

    private void AttackColliderTurnOffOn() {
        AttackColliderTurnOff();
        AttackColliderTurnOn();
    }
}
