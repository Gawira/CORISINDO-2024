using UnityEngine;

public class ObjectInteractor : MonoBehaviour
{
    public CameraSwitcher cameraSwitcher;
    public Camera playerCamera;  // Main camera
    public Camera topDownCamera; // Top-down camera

    private GameObject selectedObject;
    private Vector3 offset;

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
            Debug.Log("Left mouse button down");
            Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f); // Draw the ray for debugging
            RaycastHit hit;
            Debug.Log("Raycast from active camera");
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast hit something: " + hit.collider.gameObject.name);
                Debug.Log("Hit object tag: " + hit.collider.gameObject.tag);
                if (cameraSwitcher.IsInTopDownView() && hit.collider.CompareTag("Document"))
                {
                    Debug.Log("Document selected for dragging");
                    selectedObject = hit.collider.gameObject;
                    offset = selectedObject.transform.position - hit.point;
                }
                else if (!cameraSwitcher.IsInTopDownView() && hit.collider.CompareTag("Workstation"))
                {
                    Debug.Log("Interacting with workstation.");
                    cameraSwitcher.SwitchToTopDownView();
                }
                else
                {
                    Debug.Log("Raycast hit an object, but it is not the correct tag for this view.");
                }
            }
            else
            {
                Debug.Log("Raycast did not hit any object.");
            }
        }

        if (Input.GetMouseButton(0) && selectedObject != null)
        {
            Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 newPos = selectedObject.transform.position;
                newPos.x = hit.point.x + offset.x; // Update the X position
                newPos.z = hit.point.z + offset.z; // Update the Z position
                selectedObject.transform.position = newPos;
                Debug.Log("Document dragged to new position: " + newPos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectedObject = null;
        }

        if (Input.GetMouseButtonDown(1)) // Detect right-click
        {
            if (cameraSwitcher.CanRightClick())
            {
                Debug.Log("Right-click detected, switching to main view...");
                cameraSwitcher.SwitchToMainView();
            }
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
}
