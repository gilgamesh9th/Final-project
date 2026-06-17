using UnityEngine;
using UnityEngine.SceneManagement;
public class inGameUI : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("quit");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
        Debug.Log("ppppew");
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("Game Scene");
        Debug.Log("ppppew");
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
