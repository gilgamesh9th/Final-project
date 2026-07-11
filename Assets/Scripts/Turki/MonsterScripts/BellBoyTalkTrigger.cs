using UnityEngine;
using System.Collections;

public class BellBoyTalkTrigger : MonoBehaviour
{
    public Animator bellBoyAnimator;

    public float talkDuration = 40f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(TalkRoutine());
        }
    }

    private IEnumerator TalkRoutine()
    {
        bellBoyAnimator.SetBool("Talk", true);

        yield return new WaitForSeconds(talkDuration);

        bellBoyAnimator.SetBool("Talk", false);
    }
}