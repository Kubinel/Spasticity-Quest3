using System.Collections.Generic;

public class Quizz
{
    List<int> answerOne = new List<int>();
    List<int> answerTwo = new List<int>();
    List<int> answerThree = new List<int>();
    List<int> answerFour = new List<int>();
    List<int> answerFive = new List<int>();
    List<int> answerSix = new List<int>();
    List<int> answerSeven = new List<int>();
    List<int> answerEight = new List<int>();
    List<int> answerNine = new List<int>();
    List<int> answerTen = new List<int>();

    List<List<int>> allQuizzes = new List<List<int>>();

    public Quizz()
    {
        // answerOne.Add(-1);
        answerOne.Add(48);
        answerOne.Add(56);
        answerOne.Add(59);

        answerTwo.Add(5);
        answerTwo.Add(6);
        answerTwo.Add(8);
        answerTwo.Add(9);

        answerThree.Add(50);
        answerThree.Add(51);

        answerFour.Add(28);
        answerFour.Add(29);

        answerFive.Add(31);
        answerFive.Add(32);

        answerSix.Add(3);
        answerSix.Add(33);
        answerSix.Add(34);

        answerSeven.Add(3);
        answerSeven.Add(31);
        answerSeven.Add(32);
        answerSeven.Add(33);
        answerSeven.Add(34);

        // answerEight.Add(-1);
        answerEight.Add(5);
        answerEight.Add(6);
        answerEight.Add(8);
        answerEight.Add(9);
        answerEight.Add(48);
        answerEight.Add(56);
        answerEight.Add(59);

        // answerNine.Add(-1);
        answerNine.Add(5);
        answerNine.Add(6);
        answerNine.Add(8);
        answerNine.Add(9);
        answerNine.Add(28);
        answerNine.Add(29);
        answerNine.Add(48);
        answerNine.Add(56);
        answerNine.Add(59);

        // answerTen.Add(-1);
        answerTen.Add(5);
        answerTen.Add(6);
        answerTen.Add(8);
        answerTen.Add(9);
        answerTen.Add(31);
        answerTen.Add(32);
        answerTen.Add(48);
        answerTen.Add(50);
        answerTen.Add(51);
        answerTen.Add(56);
        answerTen.Add(59);

        allQuizzes.Add(answerOne);
        allQuizzes.Add(answerTwo);
        allQuizzes.Add(answerThree);
        allQuizzes.Add(answerFour);
        allQuizzes.Add(answerFive);
        allQuizzes.Add(answerSix);
        allQuizzes.Add(answerSeven);
        allQuizzes.Add(answerEight);
        allQuizzes.Add(answerNine);
        allQuizzes.Add(answerTen);
    }

    public List<int> GetQuizz(int index)
    {
        return allQuizzes[index];
    }
}
