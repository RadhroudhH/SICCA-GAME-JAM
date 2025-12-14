using UnityEngine;

public class KidRescueByDistance : MonoBehaviour
{
    [Header("Rescue Setup")]
    public Transform branchPile;
    public float rescueDistance = 3f;

    [Header("References")]
    public KidGuide kidGuide;
    public AudioSource cryingAudio;
    public SubtitleTypewriter subtitleTypewriter; 

    private Animator animator;
    private bool rescued = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (rescued) return;

        float distance = Vector3.Distance(transform.position, branchPile.position);

        if (distance > rescueDistance)
        {
            RescueKid();
        }
    }

    private void RescueKid()
    {
        rescued = true;

        if (cryingAudio != null)
            cryingAudio.Stop();

        if (animator != null)
            animator.SetBool("IsRescued", true);

        if (kidGuide != null)
            kidGuide.StartGuiding();

        if (subtitleTypewriter != null)
            subtitleTypewriter.ShowSubtitle("3aychou 3ammi, ija taba3ni ! Na3ref blasa no5erjou menha !");
    }
}
