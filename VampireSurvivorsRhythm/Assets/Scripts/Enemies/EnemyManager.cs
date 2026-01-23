using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance;

    public GridCellIndicator gridCellPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
    }
}