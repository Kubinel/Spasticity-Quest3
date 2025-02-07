using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [Header ("Arteries")]
    public GameObject arteries;
    public GameObject arteriesButton;
    private bool showArteries;

    [Header ("Lymphatic")]
    public GameObject lymphatic;
    public GameObject lymphaticButton;
    private bool showLymphatic;

    [Header ("MuscleAll")]
    public GameObject muscleAll;
    public GameObject muscleAllButton;
    private bool showMuscleAll;

    [Header ("Nerves")]
    public GameObject nerves;
    public GameObject nervesButton;
    private bool showNerves;

    [Header ("Skelet")]
    public GameObject skelet;
    public GameObject skeletButton;
    private bool showSkelet;

    [Header ("Skin")]
    public GameObject skin;
    public GameObject skinButton;
    private bool showSkin;

    [Header ("Veins")]
    public GameObject veins;
    public GameObject veinsButton;
    private bool showVeins;

    [Header ("Main Anatomy")]
    public GameObject main;
    public GameObject options;

    public List<GameObject> lines;
    public List<GameObject> muscleInfos;

    private void Start() 
    {
        showArteries = false;
        showLymphatic = false;
        showMuscleAll = false;
        showNerves = false;
        showSkelet = true;
        showSkin = false;
        showVeins = false;    
    }

    private void ChangeButton(GameObject button, Color newColor)
    {
        button.GetComponent<Image>().color = newColor;
    }

    public void ShowArteries()
    {
        showArteries = !showArteries;

        if (showArteries)
        {
            ChangeButton(arteriesButton, Color.blue);
            arteries.SetActive(true);
        }
        else
        {
            ChangeButton(arteriesButton, Color.white);
            arteries.SetActive(false);
        }
    }

    public void ShowLymphatic()
    {
        showLymphatic = !showLymphatic;

        if (showLymphatic)
        {
            ChangeButton(lymphaticButton, Color.blue);
            lymphatic.SetActive(true);
        }
        else
        {
            ChangeButton(lymphaticButton, Color.white);
            lymphatic.SetActive(false);
        }
    }

    public void ShowMuscleAll()
    {
        showMuscleAll = !showMuscleAll;

        if (showMuscleAll)
        {
            ChangeButton(muscleAllButton, Color.blue);
            muscleAll.SetActive(true);
        }
        else
        {
            ChangeButton(muscleAllButton, Color.white);
            muscleAll.SetActive(false);
        }
    }

    public void ShowNerves()
    {
        showNerves = !showNerves;

        if (showNerves)
        {
            ChangeButton(nervesButton, Color.blue);
            nerves.SetActive(true);
        }
        else
        {
            ChangeButton(nervesButton, Color.white);
            nerves.SetActive(false);
        }
    }

    public void ShowSkelet()
    {
        showSkelet = !showSkelet;

        if (showSkelet)
        {
            ChangeButton(skeletButton, Color.blue);
            skelet.SetActive(true);
        }
        else
        {
            ChangeButton(skeletButton, Color.white);
            skelet.SetActive(false);
        }
    }

    public void ShowSkin()
    {
        showSkin = !showSkin;

        if (showSkin)
        {
            ChangeButton(skinButton, Color.blue);
            skin.SetActive(true);
        }
        else
        {
            ChangeButton(skinButton, Color.white);
            skin.SetActive(false);
        }
    }

    public void ShowVeins()
    {
        showVeins = !showVeins;

        if (showVeins)
        {
            ChangeButton(veinsButton, Color.blue);
            veins.SetActive(true);
        }
        else
        {
            ChangeButton(veinsButton, Color.white);
            veins.SetActive(false);
        }
    }

    public void ShowMain()
    {
        main.SetActive(true);
        options.SetActive(false);
        Show(lines);
        Show(muscleInfos);
    }

    public void ShowOptions()
    {
        main.SetActive(false);
        options.SetActive(true);
        Hide(lines);
        Hide(muscleInfos);
    }

    public void AddLine(GameObject line)
    {
        lines.Add(line);
    }

    public void AddMuscleInfo(GameObject mInfo)
    {
        muscleInfos.Add(mInfo);
    }

    private void Hide(List<GameObject> list)
    {
        foreach (var item in list)
        {
            item.SetActive(false);
        }
    }
    private void Show(List<GameObject> list)
    {
        foreach (var item in list)
        {
            item.SetActive(true);
        }
    }
}
