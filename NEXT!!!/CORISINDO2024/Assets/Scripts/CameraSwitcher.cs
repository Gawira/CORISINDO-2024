using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera topDownCamera;
    public float transitionDuration = 1f;

    private bool canRightClick = false;
    private bool isInTopDownView = false;
    private bool isTransitioning = false;
    private bool canUseInput = true;

    void Start()
    {
        mainCamera.enabled = true;
        topDownCamera.enabled = false;
        mainCamera.GetComponent<AudioListener>().enabled = true;
        topDownCamera.GetComponent<AudioListener>().enabled = false;
        Debug.Log("Main camera enabled at start.");
    }

    public void SwitchToTopDownView()
    {
        if (!isInTopDownView && !isTransitioning)
        {
            Debug.Log("Switching to top-down view...");
            StartCoroutine(SmoothTransition(mainCamera, topDownCamera, true, true));
        }
    }

    public void SwitchToMainView()
    {
        if (isInTopDownView && !isTransitioning)
        {
            Debug.Log("Switching to main view...");
            topDownCamera.enabled = false;
            StartCoroutine(SmoothTransition(topDownCamera, mainCamera, false, false));
        }
    }

    private IEnumerator SmoothTransition(Camera fromCamera, Camera toCamera, bool enableRightClick, bool easeIn)
    {
        Debug.Log($"Starting smooth transition from {fromCamera.name} to {toCamera.name}...");

        Vector3 startPosition = fromCamera.transform.position;
        Quaternion startRotation = fromCamera.transform.rotation;

        Vector3 endPosition = toCamera.transform.position;
        Quaternion endRotation = toCamera.transform.rotation;

        toCamera.transform.position = startPosition;
        toCamera.transform.rotation = startRotation;
        toCamera.enabled = true;

        fromCamera.GetComponent<AudioListener>().enabled = false;

        isTransitioning = true;
        canUseInput = false;

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            float easedT = easeIn ? Mathf.SmoothStep(0, 1, t) : 1 - Mathf.SmoothStep(0, 1, 1 - t);

            toCamera.transform.position = Vector3.Lerp(startPosition, endPosition, easedT);
            toCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, easedT);

            Debug.Log($"Interpolating - Position: {toCamera.transform.position}, Rotation: {toCamera.transform.rotation.eulerAngles}");

            yield return null;
        }

        fromCamera.enabled = false;
        toCamera.GetComponent<AudioListener>().enabled = true;

        isInTopDownView = enableRightClick;
        canRightClick = enableRightClick;
        isTransitioning = false;
        Debug.Log($"Switched camera view to {toCamera.name}.");

        if (!enableRightClick)
        {
            topDownCamera.enabled = false;
        }

        ResetInputState();

        StartCoroutine(EnableInputAfterDelay(0.7f));
    }

    private void ResetInputState()
    {
        Input.ResetInputAxes();
    }

    private IEnumerator EnableInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canUseInput = true;
    }

    public bool CanRightClick()
    {
        return canRightClick;
    }

    public bool IsInTopDownView()
    {
        return isInTopDownView;
    }

    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    public bool CanUseInput()
    {
        return canUseInput;
    }
}
