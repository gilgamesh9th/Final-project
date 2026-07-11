using UnityEngine;

public class MonsterRunToPoints : MonoBehaviour
{
    public Animator animator;

    public Transform pointBathroomDoor;
    public Transform pointInsideBathroom;

    public float speed = 4f;
    public float rotateSpeed = 8f;
    public float reachDistance = 0.2f;

    private bool started = false;
    private int currentPoint = 0;

    void Start()
    {
        animator.SetBool("Run", false);
    }

    void Update()
    {
        if (!started) return;

        Transform target = currentPoint == 0 ? pointBathroomDoor : pointInsideBathroom;

        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction.magnitude <= reachDistance)
        {
            currentPoint++;

            if (currentPoint >= 2)
            {
                animator.SetBool("Run", false);
                started = false;
                return;
            }

            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    public void StartRun()
    {
        started = true;
        currentPoint = 0;
        animator.SetBool("Run", true);
    }
}