using UnityEngine;

public class MinimapObjectScript : MonoBehaviour
{
    [SerializeField] private Color[] _colors;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void ChangeColor(int arrayNumber)
    {
        if(_colors.Length <= arrayNumber) {
            Debug.LogError(gameObject.name + "の" + nameof(ChangeColor) + "に配列要素数を超えた値が指定されました");
            return;
        }

        meshRenderer.material.color = _colors[arrayNumber];
    }

}
