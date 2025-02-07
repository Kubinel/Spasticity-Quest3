using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Answer : MonoBehaviour
{
    public ObjectStorer objectStorer;
    public TMP_Text displayName;

    public List<Muscle> muscles = new List<Muscle>();
    private bool highlight;

    private bool allButtonsDisabled;

    void Start()
    {
        highlight = false;
        allButtonsDisabled = false;
    }

    public void Highlight()
    {
        if(allButtonsDisabled)
        {
            return;   
        }
        highlight = !highlight;
        if (highlight)
        {
            GetComponent<Image>().color = Color.blue; 
            displayName.GetComponent<TextMeshProUGUI>().color = Color.white;

            foreach (Muscle muscle in muscles)
            {
                objectStorer.SelectAnswer(muscle.GetPoradie());
            }
            
        }
        else
        {
            GetComponent<Image>().color = Color.white; 
            displayName.GetComponent<TextMeshProUGUI>().color = Color.black;

            foreach (Muscle muscle in muscles)
            {
                objectStorer.DeselectAnswer(muscle.GetPoradie());
            }

        }
        foreach (Muscle muscle in muscles)
        {
            muscle.Highligt();
        }

    }

    public void ShowCorrect()
    {
        GetComponent<Image>().color = Color.green; 
        foreach (Muscle muscle in muscles)
        {
            muscle.HighligtGreen();
        }
    }

    public void UnShowCorrect()
    {
        GetComponent<Image>().color = Color.green; 
        foreach (Muscle muscle in muscles)
        {
            muscle.UnHighligtGreen();
        }
    }

    public void RemoveHighlight()
    {
        highlight = false;
        
        GetComponent<Image>().color = Color.white; 
        displayName.GetComponent<TextMeshProUGUI>().color = Color.black;

        foreach (Muscle muscle in muscles)
        {
            muscle.Unhighlight();
        }
    }

    public void SetMuscle(List<Muscle> m, string s)
    {
        muscles = m;
        displayName.SetText(s);
    }

    public void SetMuscle(Muscle m, string s)
    {
        muscles.Add(m);
        displayName.SetText(s);
    }

    public void DisableButton()
    {
        allButtonsDisabled = true;
        GetComponent<Image>().color = Color.gray; 
        displayName.GetComponent<TextMeshProUGUI>().color = Color.black;
    }

    public void EnableButton()
    {
        allButtonsDisabled = false;
        GetComponent<Image>().color = Color.white;
        displayName.GetComponent<TextMeshProUGUI>().color = Color.black;
    }

    public List<int> GetMuscles()
    {
        List<int> temp = new List<int>();
        foreach (var item in muscles)
        {
            temp.Add(item.GetPoradie());
        }

        return temp;
    }
}
