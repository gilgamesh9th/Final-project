using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    public Transform teleportZone; // where to teleport the player
    public bool countsAsLoop = false; // tick ON for forward trigger only
    public bool isBackwardTrigger = false; // tick ON for backward trigger only

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // forward trigger: increment loop count
            if (countsAsLoop)
                LoopManager.Instance.OnPlayerReturned();

            // backward trigger: cancel the swap so entrance trigger wont fire
            if (isBackwardTrigger)
                LoopManager.Instance.hasCompletedLoop = false;

            // teleport the player
            Vector3 localOffset = transform.InverseTransformPoint(other.transform.position);
            Quaternion reletiveRotation = teleportZone.rotation * Quaternion.Inverse(transform.rotation);
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                other.transform.position = teleportZone.TransformPoint(localOffset);
                other.transform.rotation = reletiveRotation * other.transform.rotation;
                cc.enabled = true;
            }
        }
    }
}