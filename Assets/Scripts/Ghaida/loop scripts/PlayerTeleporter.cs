using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    public Transform destination;
    public bool countsAsLoop = false; // tick ON for end trigger only

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerTeleportCooldown.Instance.canTeleport)
        {
            PlayerTeleportCooldown.Instance.StartCooldown();

            if (countsAsLoop)
                LoopManager.Instance.OnLoopCompleted();

            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                Vector3 localOffset = transform.InverseTransformPoint(other.transform.position);
                Quaternion relativeRotation = destination.rotation * Quaternion.Inverse(transform.rotation);
                other.transform.position = destination.TransformPoint(localOffset);
                other.transform.rotation = relativeRotation * other.transform.rotation;
                cc.enabled = true;
            }
        }
    }
}