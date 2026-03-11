using System.Collections;
using UnityEngine;

public class TransparancyDetection : MonoBehaviour
{
    private const float FULL_NON_TRANSPARENT = 1.0f;

    [Range(0f, 1f)]

    [SerializeField] private float TransparancyValue = 0.7f;
    [SerializeField] private float Transition = 0.5f;

    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collider) // когда дерево пересекает
    {
        if (collider.gameObject.GetComponent<Player>()) // именно объект Player
        {
            if (collider is CapsuleCollider2D)
                StartCoroutine(FadeRoutine(_spriteRenderer, Transition, _spriteRenderer.color.a, TransparancyValue));
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>()) // именно объект Player
        {
            if (collider is CapsuleCollider2D)
                StartCoroutine(FadeRoutine(_spriteRenderer, Transition, _spriteRenderer.color.a, FULL_NON_TRANSPARENT));
        }
    }


    private IEnumerator FadeRoutine(SpriteRenderer spriteRenderer, float Transition, float startTransparancyValue, float targetTransparancyValue)
    {
        float elapsedTime = 0f; // время с начала прозрачности

        while (elapsedTime < Transition)
        {
            elapsedTime += Time.deltaTime;

            float newAlpha = Mathf.Lerp(startTransparancyValue, targetTransparancyValue, elapsedTime / Transition); // от начального к конечному
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha); // задаем новую прозрачность

            yield return null;
        }
    }

}
