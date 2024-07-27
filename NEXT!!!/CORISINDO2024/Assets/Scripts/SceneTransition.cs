using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private CanvasGroup transitionCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (transitionCanvasGroup == null)
        {
            Debug.LogError("Transition Canvas Group is not assigned.");
        }
    }

    private void Start()
    {
        // Ensure the canvas group is initially transparent
        if (transitionCanvasGroup != null)
        {
            transitionCanvasGroup.alpha = 0f;
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = transitionCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        transitionCanvasGroup.alpha = targetAlpha;
    }

    public IEnumerator FadeAndLoadScene(string sceneName)
    {
        Debug.Log("Starting fade out...");
        yield return StartCoroutine(Fade(1f));
        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(Fade(0f));
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }
}
