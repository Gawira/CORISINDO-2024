using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float rotationAngle = 90f;
    public float rotationSpeed = 2f;
    private float targetYRotation = -90f;
    private Quaternion targetRotation;
    private bool isRotating = false;
    public CameraSwitcher cameraSwitcher;
    public ObjectInteractor objectInteractor; // Reference to ObjectInteractor script

    private bool isCooldown = false; // Cooldown flag
    private float cooldownDuration = 0.4f; // Cooldown duration
    private bool hasTeleportedPassport = false; // Flag to check if the passport has been teleported

    void Start()
    {
        targetYRotation = -90f;
        targetRotation = Quaternion.Euler(0, targetYRotation, 0);
        transform.rotation = targetRotation;

        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        objectInteractor = FindObjectOfType<ObjectInteractor>(); // Initialize the ObjectInteractor reference
    }

    void Update()
    {
        if (!isRotating && cameraSwitcher.CanUseInput() && !isCooldown)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartRotation(-rotationAngle);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                StartRotation(rotationAngle);
            }
        }
        else if (isRotating)
        {
            float transitionProgress = Mathf.Clamp01(Quaternion.Angle(transform.rotation, targetRotation) / rotationSpeed);
            if (transitionProgress > 0.5f && !hasTeleportedPassport)
            {
                HandleMidTransitionViewChange(); // Call HandleMidTransitionViewChange at the midpoint of the transition
                hasTeleportedPassport = true; // Ensure this only happens once per transition
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
                StartCoroutine(Cooldown()); // Start the cooldown
                hasTeleportedPassport = false; // Reset for the next transition
            }
        }
    }

    void StartRotation(float angle)
    {
        float newRotation = targetYRotation + angle;
        if (newRotation >= -180f && newRotation <= 0f)
        {
            targetYRotation = newRotation;
            targetRotation = Quaternion.Euler(0, targetYRotation, 0);
            isRotating = true;
        }
    }

    void HandleViewChange()
    {
        if (targetYRotation == -90f) // Main view
        {
            objectInteractor.MovePassportToWorkstationTable();
        }
        else if (targetYRotation == -15f) // Right view
        {
            objectInteractor.MovePassportToRightSideTable();
        }
        // Add any other view states as needed
    }

    void HandleMidTransitionViewChange()
    {
        if (targetYRotation == -90f) // Main view
        {
            objectInteractor.MovePassportToWorkstationTable();
        }
        else if (targetYRotation == -15f) // Right view
        {
            objectInteractor.MovePassportToRightSideTable();
        }
        // Add any other view states as needed
    }

    private IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isCooldown = false;
    }
}
