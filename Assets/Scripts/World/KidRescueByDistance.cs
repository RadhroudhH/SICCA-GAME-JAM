using UnityEngine;

public class KidRescueByDistance : MonoBehaviour
{
    [Header("References")]
    public Transform branchPile;
    public KidGuide kidGuide;
    public AudioSource cryingAudio;

    [Header("Rescue Settings")]
    public float rescueDistance = 3f;

    private bool rescued = false;

    void Update()
    {
        if (rescued || branchPile == null) return;

        float distance = Vector3.Distance(transform.position, branchPile.position);

        if (distance >= rescueDistance)
        {
            RescueKid();
        }
    }

    void RescueKid()
    {
        rescued = true;

        if (cryingAudio != null)
            cryingAudio.Stop();

        kidGuide.StartGuiding();

        Debug.Log("Kid rescued by moving branches!");
    }
}
