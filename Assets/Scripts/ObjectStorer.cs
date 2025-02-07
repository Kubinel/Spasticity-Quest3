using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
// using Microsoft.MixedReality.Toolkit;
using System.Collections;

public class ObjectStorer : MonoBehaviour
{
    public List<Muscle> allMuscles = new List<Muscle>(0);
    public List<Answer> answers;
    public List<int> selectedAnswers;

    public GameObject image;
    public List<Sprite> images;

    public MuscleInfo muscleInfoObject;
    public QuizzManager quizzManager;
    public GameObject centerEyeAnchor;
    public FileRead fr;

    public TMP_Text testNumber;
    public TMP_Text sideText;

    public GameObject correctText;
    public GameObject incorrectText;

    private void Awake() 
    {
        selectedAnswers = new List<int>(0);    
    }

    private void Start() 
    {

        if(testNumber != null)
        {
            PrepareTest();
            sideText.SetText(quizzManager.GetSideNotes());
        }
    }

    private void PrepareTest()
    {
        int n = allMuscles.Count;
        bool swapped;

        do
        {
            swapped = false;
            for (int i = 1; i < n; i++)
            {
                if (string.Compare(allMuscles[i - 1].GetName(), allMuscles[i].GetName()) > 0)
                {
                    Muscle temp = allMuscles[i - 1];
                    allMuscles[i - 1] = allMuscles[i];
                    allMuscles[i] = temp;

                    swapped = true;
                }
            }
            n--;
        }
        while (swapped);

        int counter = 0;
        List<Muscle> muscles = new List<Muscle>();
        for (int i = 0; i < allMuscles.Count; i++)
        {
            if (i == 27 || i == 30)
                continue;

            if ((i == 3 || i == 4) ||
                (i >= 7 && i <= 9) ||
                (i >= 17 && i <= 20) ||
                (i == 28 || i == 29)
                )
            {
                muscles.Add(allMuscles[i]);
                if (i == 4)
                {
                    answers[counter++].SetMuscle(muscles, "Biceps");
                }
                else if (i == 9)
                {
                    answers[counter++].SetMuscle(muscles, "Extensor carpi");
                }
                else if (i == 20)
                {
                    answers[counter++].SetMuscle(muscles, "Lumbrical");
                }
                else if (i == 29)
                {
                    answers[counter++].SetMuscle(muscles, "Triceps");
                }
            }
            else
            {
                muscles = new List<Muscle>();
                Muscle m = allMuscles[i];
                answers[counter++].SetMuscle(m, m.GetName());
            }
        }
        
        testNumber.SetText("Please select " + quizzManager.GetNumberOfAnswers().ToString() + " most important muscles involved in this pattern.");
        image.GetComponent<Image>().sprite = images[quizzManager.GetNumberOfTest() - 1];
    }

    public void ClearSelected()
    {
        selectedAnswers = new List<int>(0); 
        foreach (Answer a in answers)
        {
            a.RemoveHighlight();
        }
    }

    public void CancelHighlightOnAllMuscles()
    {
        foreach (var muscle in allMuscles)
        {
            muscle.Unhighlight();
        }
    }

    public void CheckQuiz()
    {
        if (selectedAnswers.Count == 0)
        {
            StartCoroutine(ShowText(false, true));
        }
        else if(quizzManager.CheckQuiz(selectedAnswers))
        {
            StartCoroutine(ShowText(true, false));
        }
        else
        {
            StartCoroutine(ShowText(false, true));
            StartCoroutine(ShowCorrectAnswer());
        }
    }

    IEnumerator ShowText(bool correct, bool incorrect)
    {
        correctText.SetActive(correct);
        incorrectText.SetActive(incorrect);
        DisableAllButtons();

        yield return new WaitForSeconds(2);

        EnableAllButtons();
        ClearSelected();
        correctText.SetActive(false);
        incorrectText.SetActive(false);
        if(correct)
        {
            quizzManager.StartQuizz();
            sideText.SetText(quizzManager.GetSideNotes());
            testNumber.SetText("Please chose " + quizzManager.GetNumberOfAnswers().ToString() + " muscles.");
            image.GetComponent<Image>().sprite = images[quizzManager.GetNumberOfTest() - 1]; 
        }
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator ShowCorrectAnswer()
    {
        yield return new WaitForSeconds(2);
        correctText.SetActive(true);
        incorrectText.SetActive(false);
        List<int> correctAnswers = quizzManager.GetCorrectAnswers();

        foreach (var answer in answers)
        {
            foreach (var item in answer.GetMuscles())
            {
                if (!correctAnswers.Contains(item))
                {
                    continue;
                }
                answer.ShowCorrect();
            }
        }
        
        yield return new WaitForSeconds(10);

        foreach (var answer in answers)
        {
            foreach (var item in answer.GetMuscles())
            {
                if (!correctAnswers.Contains(item))
                {
                    continue;
                }
                answer.UnShowCorrect();
            }
        }
        
        ClearSelected();
        correctText.SetActive(false);
        incorrectText.SetActive(false);

        quizzManager.StartQuizz();
        sideText.SetText(quizzManager.GetSideNotes());
        testNumber.SetText("Please chose " + quizzManager.GetNumberOfAnswers().ToString() + " muscles.");
        image.GetComponent<Image>().sprite = images[quizzManager.GetNumberOfTest() - 1]; 
    }

    private void DisableAllButtons()
    {
        foreach (var answer in answers)
        {
            answer.DisableButton();
        }
    }

    private void EnableAllButtons()
    {
        foreach (var answer in answers)
        {
            answer.EnableButton();
        }
    }

    public void AddMuscle(Muscle m)
    {
        allMuscles.Add(m);
    }

    public void AddAnswer(Answer a)
    {
        answers.Add(a);
    }

    public void SelectAnswer(int a)
    {
        selectedAnswers.Add(a);
    }

    public void DeselectAnswer(int a)
    {
        selectedAnswers.Remove(a);
    }

    public MuscleInfo GetMuscleInfo()
    {
        return muscleInfoObject;
    }


    public FileRead GetFR()
    {
        return fr;
    }
 
    public GameObject GetCenterEyeAnchor()
    {
        return centerEyeAnchor;
    }
}
