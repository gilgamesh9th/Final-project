using UnityEngine;
using UnityEngine.Playables;

public class IntroCutsceneTrigger : MonoBehaviour
{
    [SerializeField] private PlayableDirector introDirector;
    [SerializeField] private Camera cutsceneCamera;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteractionII playerInteraction;

    [SerializeField] private Transform player; // the player's root Transform (with CharacterController)
    [SerializeField] private Transform cutsceneEndPoint;

    private void Start()
    {
        // Lock player, switch to cutscene camera
        playerController.enabled = false;
        playerInteraction.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerCamera.gameObject.SetActive(false);
        cutsceneCamera.gameObject.SetActive(true);

        introDirector.Play();
        introDirector.stopped += OnCutsceneEnd;
    }

    private void OnCutsceneEnd(PlayableDirector director)
    {
        // Move player to match cutscene camera's final position/facing
        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false; // must disable before teleporting, CharacterController blocks direct position sets otherwise
        player.position = cutsceneEndPoint.position;
        player.rotation = Quaternion.Euler(0f, cutsceneEndPoint.eulerAngles.y, 0f);
        cc.enabled = true;

        // Switch back to player camera, unlock movement
        cutsceneCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerController.enabled = true;
        playerInteraction.enabled = true;
    }
}