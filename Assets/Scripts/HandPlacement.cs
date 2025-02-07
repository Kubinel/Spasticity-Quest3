using UnityEngine;

public class HandPlacement : MonoBehaviour
{
    public static Vector3 position = new Vector3(0, -0.35f, 1);
    public static Vector3 scale = new Vector3(1, 1, 1);
    public static Quaternion rotation = new Quaternion(0, 180, 180, 0);

    private void OnEnable() 
    {
        transform.position = position;
        transform.localScale = scale;
        transform.rotation = rotation;
    }

    private void Update() 
    {
        position = transform.position;
        scale = transform.localScale;
        rotation = transform.rotation;
    }
}
