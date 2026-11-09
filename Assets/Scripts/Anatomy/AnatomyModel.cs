using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[DisallowMultipleComponent]
public sealed class AnatomyModel : MonoBehaviour
{
    internal const int MuscleRayLayer = 8;
    internal static readonly Color SelectedColor = new Color(0.15f, 0.65f, 1f);
    internal static readonly Color CorrectColor = new Color(0.35f, 0.95f, 0.52f);
    internal static readonly Color IncorrectColor = new Color(1f, 0.43f, 0.45f);

    [SerializeField] private GameObject focusModel;
    [SerializeField] private GameObject layerModel;
    [SerializeField] private GameObject[] layers;
    [SerializeField] private Material skinMaterial;

    private static readonly int Tint = Shader.PropertyToID("_Color");
    private MaterialPropertyBlock skinProperties;
    private readonly Dictionary<int, Muscle> muscles = new Dictionary<int, Muscle>();
    private Renderer[] skinRenderers;
    private BoxCollider grabVolume;
    private Transform pivot;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float scaleFactor = 1f;

    public static IReadOnlyList<string> LayerNames { get; } = Array.AsReadOnly(
        new[] { "Arteries", "Lymphatic", "Muscles", "Nerves", "Skeleton", "Skin", "Veins" });

    public bool HasLayers => layerModel != null;
    public IEnumerable<int> MuscleIds => muscles.Keys.OrderBy(id => id);
    public float SkinOpacity { get; private set; } = 0.67f;
    public bool LayersVisible => HasLayers && layerModel.activeSelf;
    public Transform Pivot => pivot;

    internal void Initialize(bool enableMuscleRays, Action<Muscle> select)
    {
        pivot = transform;
        foreach (var muscle in focusModel.GetComponentsInChildren<Muscle>())
        {
            if (muscle.Surface == null)
                throw new InvalidOperationException($"Muscle {muscle.Id} has no renderer.");

            muscles.Add(muscle.Id, muscle);
            if (!enableMuscleRays || !muscle.TryGetComponent(out MeshFilter filter) || filter.sharedMesh == null)
                continue;

            // Only the authored focus meshes take part in muscle picking. The model's grab box has its own layer.
            muscle.gameObject.layer = MuscleRayLayer;
            var collider = muscle.gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = filter.sharedMesh;
            var interactable = muscle.gameObject.AddComponent<XRSimpleInteractable>();
            interactable.selectEntered.AddListener(_ => select(muscle));
        }

        grabVolume = gameObject.AddComponent<BoxCollider>();
        grabVolume.isTrigger = true;
        gameObject.layer = Physics.IgnoreRaycastLayer;
        Bounds localBounds = GetLocalBounds();
        grabVolume.center = localBounds.center;
        grabVolume.size = localBounds.size + Vector3.one * 0.04f;

        if (!HasLayers)
            return;

        skinProperties = new MaterialPropertyBlock();
        skinRenderers = layers[5].GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in skinRenderers)
            renderer.sharedMaterials = renderer.sharedMaterials.Select(_ => skinMaterial).ToArray();

        for (int i = 0; i < layers.Length; i++)
            SetLayer(i, i == 4);
        SetSkinOpacity(SkinOpacity);
    }

    public Muscle GetMuscle(int id) => muscles[id];

    public void SetLayerMode(bool showLayers)
    {
        if (!HasLayers)
            return;
        focusModel.SetActive(!showLayers);
        layerModel.SetActive(showLayers);
    }

    public bool IsLayerVisible(int index) => layers[index].activeSelf;
    public void SetLayer(int index, bool visible) => layers[index].SetActive(visible);

    public void SetSkinOpacity(float opacity)
    {
        SkinOpacity = Mathf.Clamp01(opacity);
        Color color = skinMaterial.color;
        color.a = SkinOpacity;
        foreach (Renderer renderer in skinRenderers)
        {
            renderer.GetPropertyBlock(skinProperties);
            skinProperties.SetColor(Tint, color);
            renderer.SetPropertyBlock(skinProperties);
        }
    }

    public void Place(Vector3 center, Quaternion facing)
    {
        initialRotation = facing;
        initialPosition = center - facing * grabVolume.center;
        ResetPose();
    }

    public void ResetPose()
    {
        scaleFactor = 1f;
        pivot.localScale = Vector3.one;
        pivot.SetPositionAndRotation(initialPosition, initialRotation);
    }

    public bool CanGrab(Ray ray)
    {
        Vector3 center = pivot.TransformPoint(grabVolume.center);
        if ((center - ray.origin).sqrMagnitude > 16f)
            return false;
        return (grabVolume.ClosestPoint(ray.origin) - ray.origin).sqrMagnitude < 0.0225f ||
            grabVolume.Raycast(ray, out _, 4f);
    }

    public void Scale(float multiplier)
    {
        Vector3 center = pivot.TransformPoint(grabVolume.center);
        scaleFactor = Mathf.Clamp(scaleFactor * multiplier, 0.55f, 1.75f);
        pivot.localScale = Vector3.one * scaleFactor;
        pivot.position += center - pivot.TransformPoint(grabVolume.center);
    }

    private Bounds GetLocalBounds()
    {
        Bounds bounds = default;
        bool first = true;
        foreach (var renderer in focusModel.GetComponentsInChildren<MeshRenderer>())
        {
            Bounds mesh = renderer.localBounds;
            Vector3 min = mesh.min;
            Vector3 max = mesh.max;
            for (int i = 0; i < 8; i++)
            {
                Vector3 point = new Vector3(
                    (i & 1) == 0 ? min.x : max.x,
                    (i & 2) == 0 ? min.y : max.y,
                    (i & 4) == 0 ? min.z : max.z);
                point = pivot.InverseTransformPoint(renderer.transform.TransformPoint(point));
                if (first)
                {
                    bounds = new Bounds(point, Vector3.zero);
                    first = false;
                }
                else
                    bounds.Encapsulate(point);
            }
        }
        if (first)
            throw new InvalidOperationException("The anatomy model has no visible meshes.");
        return bounds;
    }
}
