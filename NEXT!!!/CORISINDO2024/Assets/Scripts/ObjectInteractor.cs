using UnityEngine;

public class ObjectInteractor : MonoBehaviour
{
    public CameraSwitcher cameraSwitcher;
    public Camera playerCamera;  // Main camera
    public Camera topDownCamera; // Top-down camera

    private GameObject selectedObject;
    private Vector3 offset;
    private float liftAmount = 0.1f; // Amount to lift the object on Y axis
    private bool isDragging = false;

    void Start()
    {
        // Ensure cameras are assigned in the inspector
        if (cameraSwitcher == null || playerCamera == null || topDownCamera == null)
        {
            Debug.LogError("Cameras or CameraSwitcher not assigned in the inspector");
        }
    }

    void Update()
    {
        if (!cameraSwitcher.CanUseInput())
        {
            return; // Disable all input during transition
        }

        Camera activeCamera = cameraSwitcher.IsInTopDownView() ? topDownCamera : playerCamera;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f); // Draw the ray for debugging
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (cameraSwitcher.IsInTopDownView() && hit.collider.CompareTag("Document"))
                {
                    SelectObject(hit.collider.gameObject, hit.point);
                    isDragging = true;
                }
                else if (!cameraSwitcher.IsInTopDownView() && hit.collider.CompareTag("Workstation"))
                {
                    cameraSwitcher.SwitchToTopDownView();
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging && selectedObject != null)
        {
            MoveSelectedObject(activeCamera);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false; // Stop dragging when the mouse button is released
        }

        if (Input.GetMouseButtonDown(1)) // Right-click to deselect and switch to main view
        {
            if (cameraSwitcher.CanRightClick())
            {
                DeselectObject();
                cameraSwitcher.SwitchToMainView();
            }
        }

        if (Input.GetKeyDown(KeyCode.X)) // Deselect the object with the X key
        {
            DeselectObject();
        }

        if (cameraSwitcher.CanUseInput() && !cameraSwitcher.IsInTopDownView())
        {
            // Handle A and D key presses for camera rotation
            if (Input.GetKeyDown(KeyCode.A))
            {
                // Rotate camera left
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                // Rotate camera right
            }
        }
    }

    void SelectObject(GameObject obj, Vector3 hitPoint)
    {
        if (selectedObject != null)
        {
            DeselectObject(); // Deselect the current object if another object is selected
        }

        selectedObject = obj;
        Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Set the object to kinematic
        }

        // Move the object up by liftAmount units on the Y axis
        Vector3 position = selectedObject.transform.position;
        position.y += liftAmount;
        selectedObject.transform.position = position;

        // Calculate offset
        offset = selectedObject.transform.position - hitPoint;
    }

    void MoveSelectedObject(Camera activeCamera)
    {
        Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 newPos = selectedObject.transform.position;
            newPos.x = hit.point.x + offset.x;
            newPos.z = hit.point.z + offset.z;
            selectedObject.transform.position = newPos;
            Debug.Log("Document dragged to new position: " + newPos);
        }
    }

    void DeselectObject()
    {
        if (selectedObject != null)
        {
            Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Set the object back to non-kinematic
            }

            // Return the object to its original Y position minus the lift amount
            Vector3 position = selectedObject.transform.position;
            position.y -= liftAmount;
            selectedObject.transform.position = position;

            selectedObject = null; // Clear the selected object
        }
    }
}
