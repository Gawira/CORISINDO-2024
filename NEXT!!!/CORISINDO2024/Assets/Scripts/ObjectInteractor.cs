using UnityEngine;
using System.Collections;

public class ObjectInteractor : MonoBehaviour
{
    public CameraSwitcher cameraSwitcher;
    public Camera playerCamera;  // Main camera
    public Camera topDownCamera; // Top-down camera
    public Transform zoomInPosition; // Transform for the zoomed-in position and rotation
    public float zoomDuration = 0.5f; // Duration for the zoom-in and zoom-out animations

    private GameObject selectedObject;
    private Vector3 offset;
    private float liftAmount = 0.1f; // Amount to lift the object on Y axis
    private bool isDragging = false;
    private bool isZoomedIn = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isAnimating = false;

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
        if (isAnimating)
        {
            return; // Disable all input during transition and animation
        }

        if (isZoomedIn)
        {
            // Disable A and D key presses for camera rotation
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(ZoomOutObject(false));
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                ImmediateDeselectFromZoom();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(ZoomOutObject(false));
            }

            // Check for left-click interactions while in zoomed-in state
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = topDownCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject != selectedObject)
                    {
                        ImmediateDeselectFromZoom();
                        SelectObject(hit.collider.gameObject, hit.point);
                        isDragging = true;
                    }
                }
            }

            return; // Prevent left-click movement while zoomed in on the selected object
        }

        if (!cameraSwitcher.CanUseInput())
        {
            return; // Disable all input during camera transition
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
                    if (selectedObject == hit.collider.gameObject)
                    {
                        isDragging = true; // Allow dragging the selected object
                        return; // Ignore if the clicked object is the same as the selected object
                    }
                    if (selectedObject != null && selectedObject != hit.collider.gameObject)
                    {
                        // Deselect the previously selected object if it is different from the new one
                        ImmediateDeselectFromZoom();
                    }
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

        if (Input.GetMouseButtonDown(1)) // Right-click to deselect or zoom out and switch to main view
        {
            if (cameraSwitcher.CanRightClick())
            {
                if (isZoomedIn)
                {
                    StartCoroutine(ZoomOutObject(false));
                }
                else
                {
                    DeselectObject();
                    cameraSwitcher.SwitchToMainView();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X)) // Deselect the object with the X key
        {
            if (isZoomedIn)
            {
                ImmediateDeselectFromZoom();
            }
            else
            {
                DeselectObject();
            }
        }

        if (Input.GetKeyDown(KeyCode.Z) && selectedObject != null) // Zoom in or out with the Z key
        {
            if (isZoomedIn)
            {
                StartCoroutine(ZoomOutObject(false));
            }
            else
            {
                StartCoroutine(ZoomInObject());
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

    void SelectObject(GameObject obj, Vector3 hitPoint)
    {
        if (selectedObject != null && selectedObject != obj)
        {
            ImmediateDeselectFromZoom(); // Deselect the current object if another object is selected
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

    void ImmediateDeselectFromZoom()
    {
        if (selectedObject != null)
        {
            isZoomedIn = false;

            // Save the current position and rotation
            originalPosition = selectedObject.transform.position;
            originalRotation = selectedObject.transform.rotation;

            // Drop the object from the zoomed-in state
            Vector3 position = selectedObject.transform.position;
            position.y -= liftAmount;
            selectedObject.transform.position = position;

            Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Set the object back to non-kinematic
            }

            selectedObject = null; // Clear the selected object
        }
    }

    IEnumerator ZoomInObject()
    {
        isAnimating = true;
        isZoomedIn = true;
        originalPosition = selectedObject.transform.position;
        originalRotation = selectedObject.transform.rotation;

        Vector3 targetPosition = zoomInPosition.position;

        // Calculate the center of the object including all children
        Vector3 objectCenter = GetObjectCenter(selectedObject);

        Vector3 zoomOffset = selectedObject.transform.position - objectCenter;

        float elapsedTime = 0;
        while (elapsedTime < zoomDuration)
        {
            selectedObject.transform.position = Vector3.Lerp(originalPosition, targetPosition + zoomOffset, (elapsedTime / zoomDuration));
            selectedObject.transform.rotation = Quaternion.Lerp(originalRotation, selectedObject.transform.rotation, (elapsedTime / zoomDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selectedObject.transform.position = targetPosition + zoomOffset;
        selectedObject.transform.rotation = selectedObject.transform.rotation; // Maintain original rotation
        yield return new WaitForSeconds(0.4f); // Delay after animation
        isAnimating = false;
    }

    IEnumerator ZoomOutObject(bool saveCurrentPosition)
    {
        isAnimating = true;
        isZoomedIn = false;

        Vector3 startPosition = selectedObject.transform.position;
        Quaternion startRotation = selectedObject.transform.rotation;

        float elapsedTime = 0;
        while (elapsedTime < zoomDuration)
        {
            selectedObject.transform.position = Vector3.Lerp(startPosition, originalPosition, (elapsedTime / zoomDuration));
            selectedObject.transform.rotation = Quaternion.Lerp(startRotation, originalRotation, (elapsedTime / zoomDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (saveCurrentPosition)
        {
            originalPosition = selectedObject.transform.position;
            originalRotation = selectedObject.transform.rotation;
        }
        else
        {
            selectedObject.transform.position = originalPosition;
            selectedObject.transform.rotation = originalRotation;
        }

        yield return new WaitForSeconds(0.4f); // Delay after animation
        isAnimating = false;
    }

    Vector3 GetObjectCenter(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return obj.transform.position;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds.center;
    }
}
