using Unity.VisualScripting;
using UnityEngine;

public class KidGuide : MonoBehaviour
{
    [Header("References")]
    public KidRescueState kidState;
    public Transform player;
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waitDistance = 2f;

    private Animator animator;
    private int currentIndex = 0;
    private bool guiding = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (kidState != null)
            kidState.OnKidRescued += StartGuiding;
    }

    private void Update()
    {
        if (!guiding || waypoints == null || waypoints.Length == 0) return;

        // Stop guiding if player is too far
        if (Vector3.Distance(player.position, transform.position) > waitDistance)
        {
            animator.SetBool("IsWalking", false);
            return;
        }

        animator.SetBool("IsWalking", true);
        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        Transform target = waypoints[currentIndex];
        Vector3 dir = (target.position - transform.position).normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

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
        currentIndex = 0;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bush"))
        {
            kidState.SetRescued();
        }
    }
}
