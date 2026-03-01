using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string IS_RUNNING = "IsRunning";
    private const string ATTACK = "Attack";

    private void Awake() // инициализируем Animator и SpriteRenderer
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        Sword.Instance.OnSwordSwing += Player_OnSwordSwing;
    }

    private void Player_OnSwordSwing(object sender, System.EventArgs e) {
        animator.SetTrigger(ATTACK);
    }

    private void Update() {
        animator.SetBool(IS_RUNNING, Player.instance.IsRunning()); // меняем значение IsRunning в аниматоре
        AdjustPlayerFacingDirection();
    }

    public void TriggerEndAttackAnimation() { // конец анимации удара
        Sword.Instance.AttackColliderTurnOff();
    }


    private void AdjustPlayerFacingDirection() // Поворачиваем PlayerVisual, т.к. к Player привязана MainCamera
    {
        Vector3 mousePos = GameInput.instance.GetMousePosition();           // получаем позицию курсора
        Vector3 playerPosition = Player.instance.GetPlayerScreenPosition(); // получаем позицию игрока

        if (mousePos.x < playerPosition.x) // если курсор левее игрока
        {
            spriteRenderer.flipX = true; // поворачиваем спрайт влево
        }
        else {
            spriteRenderer.flipX = false;
        }
    }
}
