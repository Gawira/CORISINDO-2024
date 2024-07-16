using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationAngle = 90f;
    public float rotationSpeed = 2f;
    private float targetYRotation = -90f;
    private Quaternion targetRotation;
    private bool isRotating = false;
    public CameraSwitcher cameraSwitcher;

    void Start()
    {
        targetYRotation = -90f;
        targetRotation = Quaternion.Euler(0, targetYRotation, 0);
        transform.rotation = targetRotation;

        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
    }

    void Update()
    {
        if (!isRotating && cameraSwitcher.CanUseInput())
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
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
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
}
