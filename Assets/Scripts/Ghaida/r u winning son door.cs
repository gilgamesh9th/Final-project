using UnityEngine;
using UnityEngine.Playables;

public class DoorCutscene : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private VarWrite[] enableWrites;
    [SerializeField] private VarWrite[] disableWrites;

    private bool hasPlayed = false;

    public void Interact()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        playerController.enabled = false;

        if (disableWrites == null || GameVarStore.Instance == null) return;

        foreach (var w in disableWrites)
        {
            if (w.mode == UpdateMode.Increment)
                GameVarStore.Instance.Add(w.key, w.value);
            else
                GameVarStore.Instance.Set(w.key, w.value);
        }

        director.stopped += OnCutsceneEnd;
        director.Play();
    }

    private void OnCutsceneEnd(PlayableDirector d)
    {
        director.stopped -= OnCutsceneEnd;
        playerController.enabled = true;

        if (enableWrites == null || GameVarStore.Instance == null) return;
        
        foreach (var w in enableWrites)
        {
            if (w.mode == UpdateMode.Increment)
                GameVarStore.Instance.Add(w.key, w.value);
            else
                GameVarStore.Instance.Set(w.key, w.value);
        }
    }
}