using UnityEngine;

/// <summary>
/// Spawns collectible gems when enemies die
/// </summary>
public class CollectibleSpawner : MonoBehaviour
{
    public static CollectibleSpawner Instance { get; private set; }
    
    [Header("Configuration")]
    [SerializeField] private GameObject gemPrefab;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Spawn a gem at the specified position
    /// </summary>
    public static void SpawnGem(Vector3 position, int experienceValue)
    {
        if (Instance == null || Instance.gemPrefab == null)
        {
            Debug.LogWarning("CollectibleSpawner: Cannot spawn gem - instance or prefab is null");
            return;
        }
        
        GameObject gemObj = Instantiate(Instance.gemPrefab, position, Quaternion.identity);
        
        ExperienceGem gem = gemObj.GetComponent<ExperienceGem>();
        if (gem != null)
        {
            gem.Initialize(experienceValue);
        }
    }
    
    /// <summary>
    /// Set the gem prefab
    /// </summary>
    public void SetGemPrefab(GameObject prefab)
    {
        gemPrefab = prefab;
    }
}
