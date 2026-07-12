// using UnityEngine;

// [RequireComponent(typeof(AudioSource))]
// public class DoorInteractable : MonoBehaviour, IInteractable
// {
//     [SerializeField] private Animator doorAnimator;
//     [SerializeField] private string openTriggerName = "Open";
//     [SerializeField] private string closeTriggerName = "Close";
//     [SerializeField] private AudioClip openSound;
//     [SerializeField] private AudioClip closeSound;

//     private AudioSource audioSource;
//     private bool isOpen = false;
//     private bool _locked = false;

//     private void Awake()
//     {
//         audioSource = GetComponent<AudioSource>();
//     }

//     public void Interact()
//     {
//         isOpen = !isOpen;
//         doorAnimator.SetTrigger(isOpen ? openTriggerName : closeTriggerName);
//         audioSource.PlayOneShot(isOpen ? openSound : closeSound);
//     }

//     public void CloseAndLock()
//     {
//         if (isOpen)
//         {
//             isOpen = false;
//             doorAnimator.SetTrigger(closeTriggerName);
//             audioSource.PlayOneShot(closeSound);
//         }
//         _locked = true;
//     }
// }

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTriggerName = "Open";
    [SerializeField] private string closeTriggerName = "Close";
    [SerializeField] private GameObject number;
    [SerializeField] private GameObject finalRoomnumber;

    [Header("Sound")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private AudioSource audioSource;
    private bool isOpen = false;
    private bool _locked = false;

    public bool IsLocked => _locked;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (_locked) return;

        isOpen = !isOpen;
        doorAnimator.SetTrigger(isOpen ? openTriggerName : closeTriggerName);
        audioSource.PlayOneShot(isOpen ? openSound : closeSound);
    }

    public void CloseAndLock()
    {
        if (isOpen)
        {
            isOpen = false;
            doorAnimator.SetTrigger(closeTriggerName);
            audioSource.PlayOneShot(closeSound);
        }
        _locked = true;
    }

    public void Unlock()
    {
        _locked = false;
        number.SetActive(true);
        if (finalRoomnumber != null)
            finalRoomnumber.SetActive(false);
    }
}