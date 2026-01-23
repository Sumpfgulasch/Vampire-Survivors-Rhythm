using UnityEngine;

public class GridCellIndicator : MonoBehaviour {
    [SerializeField] private Material material;
    
    public void SetEmissiveColor(Color color, float intensity) {
        material.color = color;
        material.SetColor("_EmissionColor", color * Mathf.Pow(2f, intensity));
    }
}
