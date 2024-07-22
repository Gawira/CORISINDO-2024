using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour
{
    public float endXPosition;   // Final X position of the object
    public float duration = 5.0f; // Duration of the movement
    public float returnDuration = 5.0f; // Duration before returning to the initial position

    private float startXPosition;
    private float elapsedTime = 0;
    private bool isAnimating = false;
    private bool isReturning = false; // Tracks if the object is returning to the initial position

    void Start()
    {
        startXPosition = transform.position.x;
        Debug.Log("Initial X position: " + startXPosition);
    }

    void Update()
    {
        if (isAnimating)
        {
            elapsedTime += Time.deltaTime;
            float newXPosition = Mathf.Lerp(startXPosition, endXPosition, elapsedTime / duration);
            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
            Debug.Log("Animating... Current position: " + transform.position);

            if (elapsedTime >= duration)
            {
                transform.position = new Vector3(endXPosition, transform.position.y, transform.position.z);
                isAnimating = false;
                Debug.Log("Animation ended. Final position: " + transform.position);
                StartCoroutine(ReturnToInitialPosition());
            }
        }
    }

    public void StartMoving()
    {
        if (!isAnimating && !isReturning)
        {
            isAnimating = true;
            elapsedTime = 0;
            startXPosition = transform.position.x;
            Debug.Log("Animation started. Initial X position: " + startXPosition + " End X position: " + endXPosition);
        }
    }

    private IEnumerator ReturnToInitialPosition()
    {
        isReturning = true;
        float returnElapsedTime = 0;
        float returnStartXPosition = transform.position.x;

        yield return new WaitForSeconds(returnDuration);

        while (returnElapsedTime < duration)
        {
            returnElapsedTime += Time.deltaTime;
            float newXPosition = Mathf.Lerp(returnStartXPosition, startXPosition, returnElapsedTime / duration);
            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
            Debug.Log("Returning... Current position: " + transform.position);
            yield return null;
        }

        transform.position = new Vector3(startXPosition, transform.position.y, transform.position.z);
        isReturning = false;
        Debug.Log("Return ended. Final position: " + transform.position);
    }
}