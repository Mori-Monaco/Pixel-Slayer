using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer _spriteRenderer;
    private FlashBlink _flashBlink;

    private const string IS_RUNNING = "IsRunning";
    private const string IS_DIE = "IsDie";
    private const string ATTACK = "Attack";

    private void Awake() // инициализируем Animator и SpriteRenderer
    {
        animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _flashBlink = GetComponent<FlashBlink>();
    }

    private void Start()
    {
        Sword.Instance.OnSwordSwing += Player_OnSwordSwing;
        Player.Instance.OnPlayerDeath += Instance_OnPlayerDeath;
    }

    private void Instance_OnPlayerDeath(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_DIE, true);
        _flashBlink.StopBlinking();
    }

    private void Player_OnSwordSwing(object sender, System.EventArgs e)
    {
        animator.SetTrigger(ATTACK);
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning()); // меняем значение IsRunning в аниматоре

        if (Player.Instance.IsAlive())
        {
            AdjustPlayerFacingDirection();
        }
    }

    public void TriggerEndAttackAnimation()
    { // конец анимации удара
        //Sword.Instance.AttackColliderTurnOff();
    }


    private void AdjustPlayerFacingDirection() // Поворачиваем PlayerVisual, т.к. к Player привязана MainCamera
    {
        Vector3 mousePos = GameInput.instance.GetMousePosition();           // получаем позицию курсора
        Vector3 playerPosition = Player.Instance.GetPlayerScreenPosition(); // получаем позицию игрока

        if (mousePos.x < playerPosition.x) // если курсор левее игрока
        {
            _spriteRenderer.flipX = true; // поворачиваем спрайт влево
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }

    private void OnDestroy()
    {
        Player.Instance.OnPlayerDeath -= Instance_OnPlayerDeath;
    }
}
