using UnityEngine;

public class printValues : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameVarStore.Instance.LogVarsWithValue(1);
        }
    }
}
