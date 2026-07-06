using UnityEngine;

public class MonsterTriggerStart : MonoBehaviour
{
    public MonsterRunToPoints monster;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monster.StartRun();
            gameObject.SetActive(false);
        }
    }
}