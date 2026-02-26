using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string IS_RUNNING = "IsRunning";

    private void Awake() // инициализируем Animator и SpriteRenderer
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, Player.instance.IsRunning()); // проверяем состояние героя и меняем
                                                                   // значение IsRunning в аниматоре

        AdjustPlayerFacingDirection(); // взгляд игрока в сторону курсора
    }

    // ВАЖНО: Поворачиваем именно PlayerVisual, потому что MainCamera будет привязана к Player,
    // и при повороте игрока камера не должна переворачиваться

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = GameInput.instance.GetMousePosition();           // получаем позицию курсора
        Vector3 playerPosition = Player.instance.GetPlayerScreenPosition(); // получаем позицию игрока

        if (mousePos.x < playerPosition.x) // если курсор левее игрока
        {
            spriteRenderer.flipX = true; // поворачиваем спрайт влево
        }
        else
        {
            spriteRenderer.flipX = false; 
        }
    }
}
