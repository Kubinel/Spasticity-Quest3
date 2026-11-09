using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

internal sealed class WorldPanel
{
    private static readonly Color Background = new Color(0.064f, 0.106f, 0.17f);
    private static readonly Color ButtonColor = new Color(0.18f, 0.27f, 0.39f);
    private static readonly Color Ink = new Color(0.94f, 0.97f, 1f);

    public RectTransform Root { get; }
    private float Width { get; }

    public WorldPanel(Camera camera, float width, float height)
    {
        Width = width;
        var board = new GameObject("World-space panel", typeof(RectTransform), typeof(Canvas));
        Root = (RectTransform)board.transform;
        Root.sizeDelta = new Vector2(width, height);
        Root.localScale = Vector3.one * 0.0011f;
        var canvas = board.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = camera;
        board.AddComponent<TrackedDeviceGraphicRaycaster>();
        if (Application.isEditor)
            board.AddComponent<GraphicRaycaster>();

        var background = AddImage("Background", 0, 0, width, height);
        background.color = Background;
        background.raycastTarget = false;
    }

    public RectTransform Group(string name)
    {
        return Place(new GameObject(name, typeof(RectTransform)), Root, 0, 0, Root.rect.width, Root.rect.height);
    }

    public TMP_Text Text(string text, float size, float x, float y, float width, float height, RectTransform parent = null)
    {
        var item = new GameObject("Text", typeof(RectTransform));
        Place(item, parent != null ? parent : Root, x, y, width, height);
        var label = item.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.color = Ink;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.raycastTarget = false;
        label.richText = false;
        return label;
    }

    public void Heading(string text)
    {
        Text(text, 38, 28, 20, Width - 56, 50);
    }

    public Image AddImage(string name, float x, float y, float width, float height, RectTransform parent = null)
    {
        var item = new GameObject(name, typeof(RectTransform));
        Place(item, parent != null ? parent : Root, x, y, width, height);
        return item.AddComponent<Image>();
    }

    public Button Button(string text, Action onClick, float x, float y, float width, float height,
        RectTransform parent = null, float fontSize = 22)
    {
        Image surface = AddImage(text, x, y, width, height, parent);
        surface.color = ButtonColor;
        var button = surface.gameObject.AddComponent<Button>();
        button.targetGraphic = surface;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        var label = Text(text, fontSize, 10, 2, width - 20, height - 4, surface.rectTransform);
        label.alignment = TextAlignmentOptions.Center;
        if (onClick != null)
            button.onClick.AddListener(() => onClick());
        return button;
    }

    public static void Tint(Button button, Color? color)
    {
        var graphic = (Image)button.targetGraphic;
        Color display = color ?? ButtonColor;
        graphic.color = display;
        button.GetComponentInChildren<TMP_Text>().color = color.HasValue
            ? new Color(0.02f, 0.045f, 0.08f)
            : Ink;
        ColorBlock colors = button.colors;
        colors.disabledColor = Color.white;
        button.colors = colors;
    }

    public static void SetText(Button button, string value)
    {
        button.GetComponentInChildren<TMP_Text>().SetText(value);
    }

    private static RectTransform Place(GameObject item, RectTransform parent, float x, float y, float width, float height)
    {
        var rect = (RectTransform)item.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = rect.anchorMin;
        rect.pivot = rect.anchorMin;
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, height);
        return rect;
    }
}
