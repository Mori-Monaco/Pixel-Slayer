using UnityEngine;


public class PlayerVisual : MonoBehaviour
{
    private static readonly int Die = Animator.StringToHash(IsDie);  // хэширование
    private static readonly int Running = Animator.StringToHash(IsRunning);
    private static readonly int AttackHash = Animator.StringToHash(Attack);
    
    private SpriteRenderer _spriteRenderer;
    private FlashBlink _flashBlink;
    private Animator _animator;

    private const string IsRunning = "IsRunning";
    private const string Attack = "Attack";
    private const string IsDie = "IsDie";

    private void Awake() // инициализируем Animator и SpriteRenderer
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _flashBlink = GetComponent<FlashBlink>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Player.Instance.OnPlayerDeath += Instance_OnPlayerDeath; // подписка на событи€
        Sword.Instance.OnSwordSwing += Player_OnSwordSwing;
    }

    private void Update()
    {
        _animator.SetBool(Running, Player.Instance.IsRunning()); // задаем IsRunning в аниматоре

        if (Player.Instance.IsAlive())
            AdjustPlayerFacingDirection();
    }


    private void Instance_OnPlayerDeath(object sender, System.EventArgs e)
    {
        _animator.SetBool(Die, true);
        _flashBlink.StopBlinking();
    }

    private void Player_OnSwordSwing(object sender, System.EventArgs e)
    {
        _animator.SetTrigger(AttackHash);
    }


    public void TriggerEndAttackAnimation()
    { // конец анимации удара
        //Sword.Instance.AttackColliderTurnOff();
    }


    private void AdjustPlayerFacingDirection() // ѕоворачиваем PlayerVisual, т.к. к Player прив€зана MainCamera
    {
        Vector3 mousePos = GameInput.Instance.GetMousePosition();           // получаем поз. курсора
        Vector3 playerPosition = Player.Instance.GetPlayerScreenPosition(); // получаем поз. игрока

        _spriteRenderer.flipX = mousePos.x < playerPosition.x; // если курсор левее игрока поворот = true
    }

    private void OnDestroy()
    {
        Player.Instance.OnPlayerDeath -= Instance_OnPlayerDeath;
    }
}
