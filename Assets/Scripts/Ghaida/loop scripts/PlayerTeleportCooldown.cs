using UnityEngine;

public class PlayerTeleportCooldown : MonoBehaviour
{
    public static PlayerTeleportCooldown Instance;
    public bool canTeleport = true;

    void Awake() => Instance = this;

    public void StartCooldown()
    {
        canTeleport = false;
        Invoke(nameof(Reset), 1f);
    }

    private void Reset() => canTeleport = true;
}