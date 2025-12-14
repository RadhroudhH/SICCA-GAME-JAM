using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableInteractable : Interactable
{
    [Header("Grab Settings")]
    [Tooltip("How fast this object follows the camera when grabbed")]
    public float grabFollowSpeed = 1f; // per-object multiplier

    [HideInInspector] public Rigidbody rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    public override void Interact() { }
}
