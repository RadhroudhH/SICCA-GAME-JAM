using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;
    [Header("Optional Offset from parent")]
    public Vector3 positionOffset = Vector3.zero;

    private Transform camTransform;
    private Transform parentObject;

    private void Start()
    {
        parentObject = transform.parent; // save reference to the object
        transform.SetParent(null); // detach from parent so it doesn’t inherit rotation
    }

    void LateUpdate()
    {
        if (camTransform == null)
        {
            camTransform = Camera.main?.transform;
            if (camTransform == null) return;
        }

        if (parentObject == null) return;

        // Keep position following the object
        transform.position = parentObject.position + positionOffset;

        // Compute direction to camera
        Vector3 dir = camTransform.position - transform.position;

        // Lock vertical rotation if needed
        dir.y = 0;

        // Flip direction to correct backwards rotation
        dir = -dir; // <-- flip

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }
}
