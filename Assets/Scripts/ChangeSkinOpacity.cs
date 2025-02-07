using UnityEngine;
using UnityEngine.UI;

public class ChangeSkinOpacity : MonoBehaviour
{
    public Material material;
    public Color baseColor;
    public Slider slider;
    private Renderer myRenderer;

    private void Start() 
    {
        myRenderer = GetComponent<Renderer>();
        baseColor = material.color;
        slider.value = baseColor.a;
    }

    private void Update() 
    {
        myRenderer.material.SetColor("_Color", new Color(material.color.r, material.color.g, material.color.b, slider.value));
        // baseColor = ;    
        // material.color = baseColor;
    }

    public void AddOpacity()
    {
        if (baseColor.a + 10 > 255)
        {
            baseColor = new Color(baseColor.r, baseColor.g, baseColor.b, 255);
        }
        else
        {
            baseColor = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a + 10);
        }
        slider.value = baseColor.a;
        material.color = baseColor;
    }

    public void RemoveOpacity()
    {
        if (baseColor.a - 10 < 0)
        {
            baseColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0);
        }
        else
        {
            baseColor = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a - 10);
        }
        slider.value = baseColor.a;
        material.color = baseColor;
    }
}
