using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableMask;

    private GrabbableInteractable grabbedObject;
    private GrabbableInteractable currentInteractable;

    [SerializeField] private float grabMoveForce = 10f;

    void Update()
    {
        UpdateInteraction();

        if (Input.GetKey(KeyCode.E) && currentInteractable != null)
        {
            GrabObject(currentInteractable);
        }
        else if (Input.GetKeyUp(KeyCode.E) && grabbedObject != null)
        {
            ReleaseObject();
        }
    }

    void FixedUpdate()
    {
        if (grabbedObject != null)
        {
            MoveGrabbedObject();
        }
    }

    private void UpdateInteraction()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableMask))
        {
            GrabbableInteractable interactable = hit.collider.GetComponent<GrabbableInteractable>();

            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    // Show previous hover indicator only if NOT grabbing
                    if (currentInteractable != null && grabbedObject != currentInteractable)
                        currentInteractable.ShowIndicators();

                    currentInteractable = interactable;

                    // Show indicator only if NOT grabbing
                    if (grabbedObject != currentInteractable)
                        currentInteractable.ShowIndicators();
                }
                return;
            }
        }

        // Nothing hit by ray → hide previous hover indicator if not grabbing it
        if (currentInteractable != null && grabbedObject != currentInteractable)
            currentInteractable.HideIndicators();

        currentInteractable = null;
    }

    private void GrabObject(GrabbableInteractable obj)
    {
        grabbedObject = obj;
        grabbedObject.rb.useGravity = false;
        grabbedObject.HideIndicators(); // hide while grabbing
    }

    private void ReleaseObject()
    {
        grabbedObject.rb.useGravity = true;
        grabbedObject.ShowIndicators(); // show again when released
        grabbedObject = null;
    }

    private void MoveGrabbedObject()
    {
        Vector3 targetPos = cameraTransform.position + cameraTransform.forward * 2f; // adjust distance
        Vector3 dir = (targetPos - grabbedObject.transform.position);

        float followSpeed = grabMoveForce * grabbedObject.grabFollowSpeed;
        grabbedObject.rb.linearVelocity = dir * followSpeed * Time.fixedDeltaTime;
    }
}
