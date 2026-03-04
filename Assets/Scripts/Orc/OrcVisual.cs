using Unity.VisualScripting;
using UnityEngine;

// обязательные объекты
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]

public class OrcVisual : MonoBehaviour
{


    private static readonly int Die = Animator.StringToHash(IsDie);     
    private static readonly int TakeHitHash = Animator.StringToHash(TakeHit);
    private static readonly int Running = Animator.StringToHash(IsRunning);
    private static readonly int SpeedMultiplier = Animator.StringToHash(ChasingSpeedMultiplier);
    private static readonly int AttackHash = Animator.StringToHash(Attack);

    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemyEntity enemyEntity;
    [SerializeField] private GameObject enemyShadow;

    private Animator _animator;

    private const string IsRunning = "IsRunning";
    private const string ChasingSpeedMultiplier = "ChasingSpeedMultiplier";
    private const string Attack = "Attack";
    private const string TakeHit = "TakeHit";
    private const string IsDie = "IsDie";

    SpriteRenderer _spriteRenderer;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        enemyAI.OnEnemyAttack += _enemyAI_OnEnemyAttack;
        enemyEntity.OnTakeHit += _enemyEntity_OnTakeHit;
        enemyEntity.OnDeath += _enemyEntity_OnDeath;
    }

    public void TriggerAttackAnimationTurnOff()
    {
        enemyEntity.PolygonColliderTurnOff();
    }

    public void TriggerAttackAnimationTurnOn()
    {
        enemyEntity.PolygonColliderTurnOn();
    }



    private void _enemyEntity_OnDeath(object sender, System.EventArgs e)
    {
        _animator.SetBool(Die, true);
        _spriteRenderer.sortingOrder = -1;
        enemyShadow.SetActive(false);
    }

    private void _enemyEntity_OnTakeHit(object sender, System.EventArgs e)
    {
        _animator.SetTrigger(TakeHitHash);
    }

    private void Update()
    {
        _animator.SetBool(Running, enemyAI.IsRunning);
        _animator.SetFloat(SpeedMultiplier, enemyAI.GetRoamingAnimationSpeed());
    }

    private void _enemyAI_OnEnemyAttack(object sender, System.EventArgs e)
    {
        _animator.SetTrigger(AttackHash);
    }

    private void OnDestroy()
    {
        enemyAI.OnEnemyAttack -= _enemyAI_OnEnemyAttack;
        enemyEntity.OnTakeHit -= _enemyEntity_OnTakeHit;
        enemyEntity.OnDeath -= _enemyEntity_OnDeath;
    }
}
