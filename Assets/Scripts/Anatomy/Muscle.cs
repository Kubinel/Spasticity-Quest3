using UnityEngine;

[DisallowMultipleComponent]
public sealed class Muscle : MonoBehaviour
{
    private static readonly int Tint = Shader.PropertyToID("_Color");

    // Stable ID authored on the FBX instance. It indexes the original Svaly.json.
    [SerializeField] private int poradie;

    private Renderer surface;
    private MaterialPropertyBlock properties;

    public int Id => poradie;
    public Renderer Surface => surface != null ? surface : (surface = GetComponent<Renderer>());
    public Vector3 LeaderPoint => transform.childCount > 0 ? transform.GetChild(0).position : Surface.bounds.center;
    public Vector3 LabelPoint => transform.childCount > 1 ? transform.GetChild(1).position : LeaderPoint + Vector3.up * 0.07f;

    public void SetHighlight(Color? color)
    {
        if (Surface == null || Surface.sharedMaterial == null || !Surface.sharedMaterial.HasProperty(Tint))
            return;

        if (properties == null)
            properties = new MaterialPropertyBlock();
        Surface.GetPropertyBlock(properties);
        Color original = Surface.sharedMaterial.GetColor(Tint);
        properties.SetColor(Tint, color ?? original);
        Surface.SetPropertyBlock(properties);
    }
}
