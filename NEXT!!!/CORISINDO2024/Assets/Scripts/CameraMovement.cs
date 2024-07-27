using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 2.0f;  // Kecepatan pergerakan kamera
    public float moveDuration = 5.0f;  // Durasi pergerakan kamera ke kiri
    public Camera otherCamera;  // Referensi ke kamera lain
    public Transform startPositionObject;  // Objek yang menentukan posisi awal

    private float moveTimeElapsed = 0.0f;  // Waktu yang telah berlalu sejak pergerakan dimulai
    private bool movingLeft = true;

    void Start()
    {
        // Set posisi awal kamera berdasarkan posisi startPositionObject
        transform.position = startPositionObject.position;
        UpdateCanvasSettings();
    }

    void Update()
    {
        // Gerakkan kamera
        if (movingLeft)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            moveTimeElapsed += Time.deltaTime;

            // Periksa apakah durasi pergerakan telah mencapai batas
            if (moveTimeElapsed >= moveDuration)
            {
                movingLeft = false;
                SwitchCamera();
            }
        }
    }

    void SwitchCamera()
    {
        // Aktifkan kamera lain
        otherCamera.gameObject.SetActive(true);
        otherCamera.GetComponent<CameraMovement>().StartMoving();

        // Update settings in the newly activated camera's canvas
        otherCamera.GetComponent<CameraMovement>().UpdateCanvasSettings();

        // Reset posisi dan waktu untuk pergerakan berikutnya
        transform.position = startPositionObject.position;
        moveTimeElapsed = 0.0f;

        // Nonaktifkan kamera ini
        gameObject.SetActive(false);
    }

    public void StartMoving()
    {
        movingLeft = true;
    }

    public void UpdateCanvasSettings()
    {
        AudioSettings audioSettings = GetComponentInChildren<AudioSettings>();
        GraphicsSettings graphicsSettings = GetComponentInChildren<GraphicsSettings>();

        if (audioSettings != null)
        {
            audioSettings.OnEnable(); // Reload and apply settings
        }

        if (graphicsSettings != null)
        {
            graphicsSettings.OnEnable(); // Reload and apply settings
        }
    }
}
