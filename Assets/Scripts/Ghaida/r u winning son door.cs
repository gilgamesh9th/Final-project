using UnityEngine;
using UnityEngine.Playables;

public class DoorCutscene : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private CharacterController playerController;

    private bool hasPlayed = false;

    public void Interact()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        playerController.enabled = false;

        director.stopped += OnCutsceneEnd;
        director.Play();
    }

    private void OnCutsceneEnd(PlayableDirector d)
    {
        director.stopped -= OnCutsceneEnd;
        playerController.enabled = true;
    }
}