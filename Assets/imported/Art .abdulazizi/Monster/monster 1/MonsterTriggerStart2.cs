using UnityEngine;

public class MonsterTriggerStart2 : MonoBehaviour
{
    public MonsterRunToPoints2 monster;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monster.StartRun();
            gameObject.SetActive(false);
        }
    }
}