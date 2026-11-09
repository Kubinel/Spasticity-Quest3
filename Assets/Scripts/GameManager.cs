using UnityEngine;
using UnityEngine.SceneManagement;

internal static class GameManager
{
    public static void StartQuiz() => SceneManager.LoadScene("Quiz");
    public static void StartPreview() => SceneManager.LoadScene("Preview");
    public static void ReturnToMenu() => SceneManager.LoadScene("Menu");
    public static void QuitApp() => Application.Quit();
}
