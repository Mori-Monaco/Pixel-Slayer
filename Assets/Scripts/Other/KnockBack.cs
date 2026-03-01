using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class KnockBack : MonoBehaviour
{
    [SerializeField] private float _knockBackForce = 1f;
    [SerializeField] private float _knockBackMovingTimerMax = 0.3f;

    private float _knockBackMovingTimer;

    private Rigidbody2D rb;

    public bool IsGettingKnockBack { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _knockBackMovingTimer -= Time.deltaTime;
        if (_knockBackMovingTimer < 0 )
            StopKnockBackMovement();
    }

    public void GetKnockBack(Transform damageSource)
    {
        IsGettingKnockBack = true;
        _knockBackMovingTimer = _knockBackMovingTimerMax;
        Vector2 difference = (transform.position - damageSource.position).normalized * _knockBackForce;
        rb.AddForce(difference, ForceMode2D.Impulse);
    }

    public void StopKnockBackMovement()
    {
        rb.velocity = Vector2.zero;
        IsGettingKnockBack = false;
    }
}
