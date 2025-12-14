using UnityEngine;

public class KidGuide : MonoBehaviour
{
    public Transform player;
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waitDistance = 2f;

    private Animator animator;
    private int currentIndex = 0;
    private bool guiding = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!guiding) return;

        if (Vector3.Distance(player.position, transform.position) > waitDistance)
        {
            animator.SetBool("IsWalking", false);
            return;
        }

        animator.SetBool("IsWalking", true);

        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[currentIndex];
        Vector3 dir = (target.position - transform.position).normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 5f
        );

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
            {
                guiding = false;
                animator.SetBool("IsWalking", false);
            }
        }
    }

    public void StartGuiding()
    {
        guiding = true;
    }
}
