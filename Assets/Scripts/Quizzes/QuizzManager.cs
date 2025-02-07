using UnityEngine;
using System.Collections.Generic;
using System;

public class QuizzManager : MonoBehaviour
{
    public ObjectStorer objectStorer;

    private Quizz quizzes;
    private List<int> quiz;
    private  List<int> doneQuizzes;
    private int currentQuiz;

    void Start()
    {
        doneQuizzes = new List<int>(0);
        quizzes = new Quizz();
        StartQuizz();
    }

    public void StartQuizz()
    {
        do
        {
            currentQuiz = UnityEngine.Random.Range(1,11);
        } while (CheckIfInList(currentQuiz, doneQuizzes));

        doneQuizzes.Add(currentQuiz);

        if (doneQuizzes.Count == 10)
        {
            doneQuizzes = new List<int>(0);
        }

        quiz = quizzes.GetQuizz(currentQuiz - 1);
    }

    public bool CheckQuiz(List<int> answers)
    {
        if (answers == null)
            return false;
        answers.Sort();
        for (int i = 0; i < quiz.Count; i++)
        {
            if (quiz[i] != answers[i])
            {
                return false;
            }
        }

        return true;
    }

    public List<int> GetCorrectAnswers()
    {
        return quiz;
    }

    private bool CheckIfInList(int n, List<int> list)
    {
        foreach (int num in list)
        {
            if (num == n)
                return true;
        }
        return false;
    }

    public int GetNumberOfTest()
    {
        return currentQuiz;
    }

    public String GetSideNotes()
    {
        if (currentQuiz == 1)
        {
            return "-Abducted shoulder \n-Internal rotation";
        }
        else if (currentQuiz == 2)
        {
            return "-Flexed elbow";
        }
        else if (currentQuiz == 3)
        {
            return "-Pronated forearm";
        }
        else if (currentQuiz == 4)
        {
            return "-Flexed wrist";
        }
        else if (currentQuiz == 5)
        {
            return "-Flexed fingers";
        }
        else if (currentQuiz == 6)
        {
            return "-Thumb in palm";
        }
        else if (currentQuiz == 7)
        {
            return "-Clenched fist";
        }
        else if (currentQuiz == 8)
        {
            return "-Abducted shoulder \n-Flexed elbow";
        }
        else if (currentQuiz == 9)
        {
            return "-Abducted shoulder \n-Flexed elbow \n-Suspinated forearm \n-Flexed wrist";
        }
        else
        {
            return "-Abducted shoulder \n-Flexed elbow \n-Prorated forearm \n-Flexed wrist";
        }
    }

    public int GetNumberOfAnswers()
    {
        if(CheckForBiceps())
        {
            return quiz.Count - 1;
        }
        else
        {
            return quiz.Count;
        }
    }

    private bool CheckForBiceps()
    {
        foreach (var item in quiz)
        {
            if (item == 5)
            {
                return true;
            }
        }
        return false;
    }
}
