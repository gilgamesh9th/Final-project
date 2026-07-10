using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneEndLoader : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void OnEnable()
    {
        director.stopped += OnCutsceneFinished;
    }

    private void OnDisable()
    {
        director.stopped -= OnCutsceneFinished;
    }

    private void OnCutsceneFinished(PlayableDirector pd)
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}