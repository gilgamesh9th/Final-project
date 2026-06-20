using UnityEngine;

public class playerteleporter : MonoBehaviour
{

    public Transform teleportZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("triggerd" + other.name);
            Vector3 localOffset = transform.InverseTransformPoint(other.transform.position);//same player positon
            Quaternion reletiveRotation = teleportZone.rotation * Quaternion.Inverse(transform.rotation);

            CharacterController cc = other.GetComponent<CharacterController>();

            if (cc != null)// disable player controller
            {
                cc.enabled = false;
                other.transform.position = teleportZone.TransformPoint(localOffset);
                other.transform.rotation = reletiveRotation * other.transform.rotation;
                cc.enabled = true;
            }
        }
    }
}

