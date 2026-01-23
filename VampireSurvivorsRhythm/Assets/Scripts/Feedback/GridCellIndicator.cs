using UnityEngine;

public class GridCellIndicator : MonoBehaviour {
    [SerializeField] private Material material;
    
    public Vector2 GridPosition { get; set; }
    
    public void SetEmissiveColor(Color color, float intensity) {
        if (material == null) {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null) {
                material = renderer.material;
            }
        }
        
        if (material != null) {
            material.color = color;
            material.SetColor("_EmissionColor", color * Mathf.Pow(2f, intensity));
        }
    }
    
    private void Start() {
        // Store grid position from world position
        GridPosition = new Vector2(transform.position.x, transform.position.z);
    }
}
