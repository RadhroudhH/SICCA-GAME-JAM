using UnityEngine;

public class ExitBushGate : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private KidRescueState kidState;
    [SerializeField] private Collider blockingCollider; // Collider that blocks the player

    private void Start()
    {
        if (blockingCollider != null)
            blockingCollider.enabled = true;

        if (kidState != null)
            kidState.OnKidRescued += UnlockGate;
    }

    private void UnlockGate()
    {
        if (blockingCollider != null)
            blockingCollider.enabled = false;

        Debug.Log("Gate unlocked, player can pass!");
    }
}
