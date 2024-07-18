using UnityEngine;
using System.Collections;

public class ObjectInteractor : MonoBehaviour
{
    public CameraSwitcher cameraSwitcher;
    public Camera playerCamera;
    public Camera topDownCamera;
    public Transform zoomInPosition;
    public Transform batEquipPosition; // Reference to the BatEquipPosition GameObject
    public Transform batHandlerPosition; // Reference to the BatHandlerPosition GameObject
    public float zoomDuration = 0.5f;

    private GameObject selectedObject;
    private Vector3 offset;
    private float liftAmount = 0.1f;
    private bool isDragging = false;
    private bool isZoomedIn = false;
    private bool isEquipped = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale; // Store original scale of selected object
    private GameObject bat; // Reference to the currently equipped bat GameObject
    private Vector3 batOriginalPosition;
    private Quaternion batOriginalRotation;
    private Vector3 batOriginalScale;
    private bool isAnimating = false;

    void Start()
    {
        if (cameraSwitcher == null || playerCamera == null || topDownCamera == null || batEquipPosition == null || batHandlerPosition == null)
        {
            Debug.LogError("Cameras, CameraSwitcher, BatEquipPosition, or BatHandlerPosition not assigned in the inspector");
        }
    }

    void Update()
    {
        if (isAnimating)
        {
            return;
        }

        if (isZoomedIn)
        {
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

            return;
        }

        if (!cameraSwitcher.CanUseInput())
        {
            return;
        }

        Camera activeCamera = cameraSwitcher.IsInTopDownView() ? topDownCamera : playerCamera;

        if (Input.GetMouseButtonDown(0) && !isEquipped)
        {
            Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
                if (cameraSwitcher.IsInTopDownView() && hit.collider.CompareTag("Document"))
                {
                    if (selectedObject == hit.collider.gameObject)
                    {
                        isDragging = true;
                        return;
                    }
                    if (selectedObject != null && selectedObject != hit.collider.gameObject)
                    {
                        ImmediateDeselectFromZoom();
                    }
                    SelectObject(hit.collider.gameObject, hit.point);
                    isDragging = true;
                }
                else if (!cameraSwitcher.IsInTopDownView() && hit.collider.CompareTag("Workstation"))
                {
                    cameraSwitcher.SwitchToTopDownView();
                }
                else if (hit.collider.CompareTag("Bat"))
                {
                    Debug.Log("Bat clicked!");
                    EquipBat(hit.collider.gameObject);
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging && selectedObject != null)
        {
            MoveSelectedObject(activeCamera);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (Input.GetMouseButtonDown(1))
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

            Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("BatHandler"))
                {
                    UnequipBat();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
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

        if (Input.GetKeyDown(KeyCode.Z) && selectedObject != null)
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
            ImmediateDeselectFromZoom();
        }

        selectedObject = obj;
        Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Vector3 position = selectedObject.transform.position;
        position.y += liftAmount;
        selectedObject.transform.position = position;

        offset = selectedObject.transform.position - hitPoint;

        // Store original position, rotation, and scale of the selected object
        originalPosition = selectedObject.transform.position;
        originalRotation = selectedObject.transform.rotation;
        originalScale = selectedObject.transform.localScale;
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
                rb.isKinematic = false;
            }

            Vector3 position = selectedObject.transform.position;
            position.y -= liftAmount;
            selectedObject.transform.position = position;

            selectedObject = null;
        }
    }

    void ImmediateDeselectFromZoom()
    {
        if (selectedObject != null)
        {
            isZoomedIn = false;

            originalPosition = selectedObject.transform.position;
            originalRotation = selectedObject.transform.rotation;

            Vector3 position = selectedObject.transform.position;
            position.y -= liftAmount;
            selectedObject.transform.position = position;

            Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            selectedObject = null;
        }
    }

    IEnumerator ZoomInObject()
    {
        isAnimating = true;
        isZoomedIn = true;
        originalPosition = selectedObject.transform.position;
        originalRotation = selectedObject.transform.rotation;

        Vector3 targetPosition = zoomInPosition.position;

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
        selectedObject.transform.rotation = selectedObject.transform.rotation;
        yield return new WaitForSeconds(0.4f);
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

        yield return new WaitForSeconds(0.4f);
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

    void EquipBat(GameObject batObject)
    {
        if (isEquipped)
        {
            Debug.Log("Bat is already equipped.");
            return;
        }

        isEquipped = true;

        // Store the original position, rotation, and scale of the bat
        batOriginalPosition = batObject.transform.position;
        batOriginalRotation = batObject.transform.rotation;
        batOriginalScale = batObject.transform.localScale;

        // Parent the bat to the batEquipPosition
        batObject.transform.SetParent(batEquipPosition);

        // Set the local position, rotation, and scale of the bat to match the batEquipPosition
        batObject.transform.localPosition = Vector3.zero;
        batObject.transform.localRotation = Quaternion.identity;
        batObject.transform.localScale = Vector3.one; // Ensure consistent scale

        // Assign the batObject to the local variable bat
        bat = batObject;

        Debug.Log("Bat equipped. Scale: " + batObject.transform.localScale);
    }

    void UnequipBat()
    {
        if (!isEquipped)
        {
            Debug.Log("No bat is currently equipped.");
            return;
        }

        isEquipped = false;

        // Parent the bat back to the batHandlerPosition
        bat.transform.SetParent(batHandlerPosition);

        // Restore the original position, rotation, and scale of the bat
        bat.transform.position = batOriginalPosition;
        bat.transform.rotation = batOriginalRotation;
        bat.transform.localScale = batOriginalScale; // Restore the original scale

        Debug.Log("Bat unequipped. Scale: " + bat.transform.localScale);
    }
}
