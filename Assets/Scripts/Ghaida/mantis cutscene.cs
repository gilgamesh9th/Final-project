using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    public PlayableDirector director;
    public GameObject player;
    public CinemachineCamera cutsceneCam;

    public PlayerController playerController;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerController.enabled = false;
      
        director.Play();
        director.stopped += OnCutsceneEnd;
    }

    void OnCutsceneEnd(PlayableDirector d)
    {
        director.stopped -= OnCutsceneEnd;

        player.transform.position = cutsceneCam.transform.position;
        player.transform.rotation = Quaternion.Euler(0, cutsceneCam.transform.eulerAngles.y, 0);
        playerController.enabled = true;
      
     
    }
}