using UnityEngine;

public class DestructiblePlantVisual : MonoBehaviour
{
    [SerializeField] private DestructiblePlant _destructiblePlant;
    [SerializeField] private GameObject _rockDeathVFXPrefab;

    private void Start()
    {
        _destructiblePlant.OnDestructibleTakeDamage += DestructiblePlant_OnDestructibleTakeDamage;
    }

    private void DestructiblePlant_OnDestructibleTakeDamage(object sender, System.EventArgs e)
    {
        ShowDeathVFX();
    }

    private void ShowDeathVFX()
    {
        Instantiate(_rockDeathVFXPrefab, _destructiblePlant.transform.position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        _destructiblePlant.OnDestructibleTakeDamage -= DestructiblePlant_OnDestructibleTakeDamage;
    }

}
