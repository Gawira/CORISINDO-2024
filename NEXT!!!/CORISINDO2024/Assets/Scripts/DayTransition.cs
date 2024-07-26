using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DayTransition : MonoBehaviour
{
    public CanvasGroup transitionCanvasGroup;
    public float fadeDuration = 1f;

    private void Start()
    {
        StartCoroutine(FadeFromBlackToWhite());
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        transitionCanvasGroup.alpha = endAlpha;
    }

    private IEnumerator FadeFromBlackToWhite()
    {
        // Start with black
        transitionCanvasGroup.alpha = 1f;
        transitionCanvasGroup.GetComponent<Image>().color = Color.black;

        // Fade to white
        yield return StartCoroutine(Fade(1f, 0f));
    }
}
