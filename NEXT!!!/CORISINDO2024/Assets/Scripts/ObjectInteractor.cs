using System.Collections;
using UnityEngine;

public class ObjectInteractor : MonoBehaviour
{
    public CameraSwitcher cameraSwitcher;
    public Camera playerCamera;
    public Camera topDownCamera;
    public Transform zoomInPosition;
    public Transform batEquipPosition; // Reference to the BatEquipPosition GameObject
    public Transform batHandlerPosition; // Reference to the BatHandlerPosition GameObject
    public Transform workstationTablePosition; // Reference to the Workstation Table Position
    public Transform rightSideTablePosition; // Reference to the Right Side Table Position
    public float zoomDuration = 0.5f;
    public float attackDuration = 0.5f; // Duration of the attack animation
    public float equipDelay = 0.8f; // Delay before the user can attack after equipping

    public AudioSource attackAudioSource; // AudioSource for the bat attack sound
    public AudioSource hitAudioSource; // AudioSource for the hit sound

    private GameObject selectedObject;
    private GameObject passportObject; // Reference to the passport object
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
    private bool canAttack = false; // Flag to check if the user can attack
    private bool isCooldown = false; // Cooldown for spamming prevention

    private int viewState = 0; // -1 for left view, 0 for main view, 1 for right view
    private bool isRaycastEnabled = true; // Flag to control raycasting


    void Start()
    {
        if (cameraSwitcher == null || playerCamera == null || topDownCamera == null || batEquipPosition == null || batHandlerPosition == null || workstationTablePosition == null || rightSideTablePosition == null)
        {
            Debug.LogError("Cameras, CameraSwitcher, BatEquipPosition, BatHandlerPosition, WorkstationTablePosition, or RightSideTablePosition not assigned in the inspector");
        }

        UpdatePassportReference();
    }

    void Update()
    {
        if (isAnimating || isCooldown || !isRaycastEnabled)
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

            // Add rotation functionality
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                float rotationSpeed = 100f; // Adjust rotation speed as needed
                float rotationInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

                Vector3 center = CalculateCenter(selectedObject);
                RotateAround(selectedObject, center, Vector3.up, -rotationInput);
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

                // Allow button interaction in both top-down and main camera views
                if (hit.collider.CompareTag("Button"))
                {
                    Debug.Log("Button clicked!");
                    SimpleButton button = hit.collider.GetComponent<SimpleButton>();
                    if (button != null)
                    {
                        Debug.Log("SimpleButton component found!");
                        button.PressButton(); // Call the method to trigger the event
                    }
                    else
                    {
                        Debug.LogError("SimpleButton component not found on the hit object.");
                        LogHitObjectHierarchy(hit.collider.gameObject); // Log the hierarchy and components
                    }
                }
                else if (cameraSwitcher.IsInTopDownView() && hit.collider.CompareTag("Document"))
                {
                    // Handle document selection in top-down view...
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
                    StartCoroutine(EquipBat(hit.collider.gameObject));
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

        // Handle bat attack animation
        if (Input.GetMouseButtonDown(0) && isEquipped && canAttack)
        {
            StartCoroutine(PerformBatAttack());
        }
    }

    void LogHitObjectHierarchy(GameObject hitObject)
    {
        Debug.Log("Hit object hierarchy:");
        Transform current = hitObject.transform;
        while (current != null)
        {
            Debug.Log(current.name);
            current = current.parent;
        }

        Debug.Log("Components on hit object:");
        foreach (var component in hitObject.GetComponents<Component>())
        {
            Debug.Log(component.GetType().Name);
        }
    }

    void SelectObject(GameObject obj, Vector3 hitPoint)
    {
        if (obj.CompareTag("Document"))
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
    }


    void MoveSelectedObject(Camera activeCamera)
    {
        if (selectedObject != null && selectedObject.CompareTag("Document"))
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
    }

    void DeselectObject()
    {
        if (selectedObject != null && selectedObject.CompareTag("Document"))
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

    IEnumerator EquipBat(GameObject batObject)
    {
        if (isEquipped)
        {
            Debug.Log("Bat is already equipped.");
            yield break;
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

        // Delay before allowing the user to attack
        yield return new WaitForSeconds(equipDelay);
        canAttack = true;
    }

    void UnequipBat()
    {
        if (!isEquipped)
        {
            Debug.Log("No bat is currently equipped.");
            return;
        }

        isEquipped = false;
        canAttack = false; // Disable attacking when the bat is unequipped

        // Parent the bat back to the batHandlerPosition
        bat.transform.SetParent(batHandlerPosition);

        // Restore the original position, rotation, and scale of the bat
        bat.transform.position = batOriginalPosition;
        bat.transform.rotation = batOriginalRotation;
        bat.transform.localScale = batOriginalScale; // Restore the original scale

        Debug.Log("Bat unequipped. Scale: " + bat.transform.localScale);
    }

    public class EasingFunctions
    {
        public static float EaseInOut(float t)
        {
            return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
        }
    }

    IEnumerator PerformBatAttack()
    {
        isAnimating = true;

        // Define the attack positions and rotations
        Vector3 attackStartPos = bat.transform.localPosition;
        Quaternion attackStartRot = bat.transform.localRotation;

        // Adjusted pull-back position and rotation
        Vector3 attackPullBackPos = new Vector3(attackStartPos.x, attackStartPos.y, attackStartPos.z - 0.3f);  // Pull back slightly on the Z axis
        Quaternion attackPullBackRot = Quaternion.Euler(attackStartRot.eulerAngles.x - 20f, attackStartRot.eulerAngles.y - 20f, attackStartRot.eulerAngles.z);

        // Adjusted attack end position and rotation for a forward and left swing
        Vector3 attackEndPos = new Vector3(attackStartPos.x, attackStartPos.y, attackStartPos.z + 0.5f); // Swing forward on the Z axis
        Quaternion attackEndRot = Quaternion.Euler(attackStartRot.eulerAngles.x + 80f, attackStartRot.eulerAngles.y - 40f, attackStartRot.eulerAngles.z);

        int steps = 10; // Increase the number of steps for smoother animation

        // Play the attack sound
        attackAudioSource.Play();

        // Pull-back phase
        for (int i = 0; i < steps; i++)
        {
            float t = (float)i / steps;
            float easedT = EasingFunctions.EaseInOut(t);
            bat.transform.localPosition = Vector3.Lerp(attackStartPos, attackPullBackPos, easedT);
            bat.transform.localRotation = Quaternion.Lerp(attackStartRot, attackPullBackRot, easedT);
            yield return null; // Wait for the next frame
        }

        bat.transform.localPosition = attackPullBackPos;
        bat.transform.localRotation = attackPullBackRot;

        // Swing phase
        for (int i = 0; i < steps; i++)
        {
            float t = (float)i / steps;
            float easedT = EasingFunctions.EaseInOut(t);
            bat.transform.localPosition = Vector3.Lerp(attackPullBackPos, attackEndPos, easedT);
            bat.transform.localRotation = Quaternion.Lerp(attackPullBackRot, attackEndRot, easedT);
            yield return null; // Wait for the next frame
        }

        bat.transform.localPosition = attackEndPos;
        bat.transform.localRotation = attackEndRot;

        // Return to original position and rotation
        for (int i = 0; i < steps; i++)
        {
            float t = (float)i / steps;
            float easedT = EasingFunctions.EaseInOut(t);
            bat.transform.localPosition = Vector3.Lerp(attackEndPos, attackStartPos, easedT);
            bat.transform.localRotation = Quaternion.Lerp(attackEndRot, attackStartRot, easedT);
            yield return null; // Wait for the next frame
        }

        bat.transform.localPosition = attackStartPos;
        bat.transform.localRotation = attackStartRot;

        isAnimating = false;
    }

    public void PlayHitSound()
    {
        hitAudioSource.Play();
    }

    public void MovePassportToWorkstationTable()
    {
        if (passportObject != null)
        {
            passportObject.transform.position = workstationTablePosition.position;
            passportObject.transform.rotation = Quaternion.Euler(0, 0, 0); // Reset Y rotation to 0
        }
    }

    public void MovePassportToRightSideTable()
    {
        if (passportObject != null)
        {
            passportObject.transform.position = rightSideTablePosition.position;
            passportObject.transform.rotation = Quaternion.Euler(0, 90, 0); // Set Y rotation to 90
        }
    }

    public void LabelPassport(string status)
    {
        if (passportObject != null)
        {
            Debug.Log("Passport labeled as: " + status);
            // Here you can add more logic to visually indicate the label, such as applying a stamp
        }
    }

    public void UpdatePassportReference()
    {
        passportObject = FindObjectByIdentifier<PassportIdentifier>();
        if (passportObject == null)
        {
            Debug.LogError("Passport object not found in the scene");
        }
    }

    private GameObject FindObjectByIdentifier<T>() where T : Component
    {
        T[] objects = GameObject.FindObjectsOfType<T>();
        if (objects.Length > 0)
        {
            return objects[0].gameObject; // Return the first object found with the identifier component
        }
        return null;
    }

    public bool IsPassportOnRightSideTable()
    {
        if (passportObject != null)
        {
            float distance = Vector3.Distance(passportObject.transform.position, rightSideTablePosition.position);
            Debug.Log("Passport Position: " + passportObject.transform.position);
            Debug.Log("Right Side Table Position: " + rightSideTablePosition.position);
            Debug.Log("Distance: " + distance);
            return distance < 0.2f; // Adjust this threshold as needed
        }
        return false;
    }

    Vector3 CalculateCenter(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return obj.transform.position;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds.center;
    }

    void RotateAround(GameObject obj, Vector3 point, Vector3 axis, float angle)
    {
        obj.transform.RotateAround(point, axis, angle);
    }

    // Method to enable or disable raycasting
    public void SetRaycastEnabled(bool enabled)
    {
        isRaycastEnabled = enabled;
    }
}
