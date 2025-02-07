using TMPro;
using UnityEngine;

public class Choice : MonoBehaviour
{
    public TMP_Text titleObject;


    public void SetTitle(string title)
    {
        titleObject.SetText(title);
    }
}
