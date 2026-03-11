using UnityEngine;
using TMPro; // если используете TextMeshPro, иначе замените на UnityEngine.UI

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        if (Player.Instance != null)
        {
            // ѕодписываемс€ на событи€ изменени€ здоровь€ и смерти
            Player.Instance.OnHealthChanged += UpdateHealth;
            Player.Instance.OnPlayerDeath += OnPlayerDeath;

            // ѕервоначальное обновление
            UpdateHealth(null, System.EventArgs.Empty);
        }
        else
        {
            Debug.LogError("Player.Instance не найден!");
        }
    }

    private void UpdateHealth(object sender, System.EventArgs e)
    {
        if (healthText != null)
        {
            // ќтображаем только текущее здоровье
            healthText.text = "HP: " + Player.Instance.CurrentHealth.ToString();

            // ћожно добавить и максимальное: "HP: 7/10"
            // healthText.text = $"HP: {Player.Instance.CurrentHealth}/{Player.Instance.MaxHealth}";
        }
    }

    private void OnPlayerDeath(object sender, System.EventArgs e)
    {
        if (healthText != null)
        {
            healthText.text = "HP: 0"; // или "Dead"
        }
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnHealthChanged -= UpdateHealth;
            Player.Instance.OnPlayerDeath -= OnPlayerDeath;
        }
    }
}