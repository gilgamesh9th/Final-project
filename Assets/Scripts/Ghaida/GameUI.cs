using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private InputActionReference pauseAction;

    public static bool IsPaused { get; private set; }

    private void OnEnable()
    {
        pauseAction.action.performed += OnPausePressed;
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePressed;
        pauseAction.action.Disable();
    }

    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (!IsPaused) Pause();
    }

    public void Pause()
    {
        IsPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        IsPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("quit");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
        Debug.Log("ppppew");
    }

    public void LoadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game Scene");
        Debug.Log("ppppew");
    }
}