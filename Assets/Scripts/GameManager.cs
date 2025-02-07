using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void QuitApp()
    {
        Application.Quit();
    }

    public void StratQuiz()
    {
        SceneManager.LoadScene("Quiz");
    }

    public void StartPreview()
    {
        SceneManager.LoadScene("Preview");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
