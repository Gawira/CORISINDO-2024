using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera topDownCamera;
    public float transitionDuration = 1f;

    private bool canRightClick = false;
    private bool isInTopDownView = false;

    void Start()
    {
        // Ensure only the main camera is active at the start
        mainCamera.enabled = true;
        topDownCamera.enabled = false;
        mainCamera.GetComponent<AudioListener>().enabled = true;
        topDownCamera.GetComponent<AudioListener>().enabled = false;
        Debug.Log("Main camera enabled at start.");
    }

    public void SwitchToTopDownView()
    {
        if (!isInTopDownView)
        {
            Debug.Log("Switching to top-down view...");
            StartCoroutine(SmoothTransition(mainCamera, topDownCamera, true));
        }
    }

    public void SwitchToMainView()
    {
        if (isInTopDownView)
        {
            Debug.Log("Switching to main view...");
            topDownCamera.enabled = false; // Disable the camera component immediately
            StartCoroutine(SmoothTransition(topDownCamera, mainCamera, false));
        }
    }

    private IEnumerator SmoothTransition(Camera fromCamera, Camera toCamera, bool enableRightClick)
    {
        Debug.Log($"Starting smooth transition from {fromCamera.name} to {toCamera.name}...");

        Vector3 startPosition = fromCamera.transform.position;
        Quaternion startRotation = fromCamera.transform.rotation;

        Vector3 endPosition = toCamera.transform.position;
        Quaternion endRotation = toCamera.transform.rotation;

        // Enable the 'to' camera to set its initial position and rotation
        toCamera.transform.position = startPosition;
        toCamera.transform.rotation = startRotation;
        toCamera.enabled = true;

        // Disable audio listener on the from camera
        fromCamera.GetComponent<AudioListener>().enabled = false;

        // Disable right click during transition
        canRightClick = false;

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            toCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            toCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

            Debug.Log($"Interpolating - Position: {toCamera.transform.position}, Rotation: {toCamera.transform.rotation.eulerAngles}");

            yield return null;
        }

        // Disable the 'from' camera and enable audio listener on the 'to' camera
        fromCamera.enabled = false;
        toCamera.GetComponent<AudioListener>().enabled = true;

        isInTopDownView = enableRightClick;
        canRightClick = enableRightClick;
        Debug.Log($"Switched camera view to {toCamera.name}.");

        if (!enableRightClick)
        {
            // Ensure the top-down camera component is disabled when switching back to the main camera
            topDownCamera.enabled = false;
        }
    }

    public bool CanRightClick()
    {
        return canRightClick;
    }

    public bool IsInTopDownView()
    {
        return isInTopDownView;
    }
}
