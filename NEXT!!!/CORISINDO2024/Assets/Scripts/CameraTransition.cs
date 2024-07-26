using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraTransition : MonoBehaviour
{
    public Image transitionPanel;  // Referensi ke panel transisi
    public float fadeDuration = 1.0f;  // Durasi fade in/out
    public Camera mainCamera;  // Referensi ke kamera utama

    void Start()
    {
        if (transitionPanel != null)
        {
            // Set initial alpha to 0 (fully transparent)
            transitionPanel.color = new Color(0, 0, 0, 0);
        }
    }

    public void SwitchCamera(Camera newCamera)
    {
        StartCoroutine(FadeOutIn(newCamera));
    }

    private IEnumerator FadeOutIn(Camera newCamera)
    {
        yield return StartCoroutine(Fade(1));  // Fade out

        // Switch cameras
        mainCamera.gameObject.SetActive(false);
        newCamera.gameObject.SetActive(true);
        newCamera.GetComponent<CameraMovement>().StartMoving();

        yield return StartCoroutine(Fade(0));  // Fade in
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = transitionPanel.color.a;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            transitionPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Ensure the final alpha is set
        transitionPanel.color = new Color(0, 0, 0, targetAlpha);
    }
}
