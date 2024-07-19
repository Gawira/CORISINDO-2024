using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float rightXPosition;  // Posisi x ke kanan
    public float leftXPosition;   // Posisi x ke kiri
    public float duration = 5.0f; // Durasi perpindahan

    private float startXPosition;
    private float targetXPosition;
    private float elapsedTime = 0;
    private bool isAnimating = false;
    private bool moveRight = true; // Default gerakan ke kanan

    void Start()
    {
        // Inisialisasi posisi awal objek
        startXPosition = transform.position.x;
        Debug.Log("Starting x position: " + startXPosition);
    }

    void Update()
    {
        // Mulai animasi dengan menekan tombol (misalnya, tombol 'Space')
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isAnimating = true;
            elapsedTime = 0; // Reset waktu
            startXPosition = transform.position.x; // Set start x position ketika animasi dimulai
            targetXPosition = moveRight ? rightXPosition : leftXPosition; // Tentukan posisi target berdasarkan arah gerakan
            Debug.Log("Animation started. Start x position: " + startXPosition + " Target x position: " + targetXPosition);
        }

        // Ubah arah gerakan dengan menekan tombol (misalnya, tombol 'R')
        if (Input.GetKeyDown(KeyCode.R))
        {
            moveRight = !moveRight; // Balikkan arah gerakan
            Debug.Log("Direction changed. Move right: " + moveRight);
        }

        // Jika animasi sedang berjalan
        if (isAnimating)
        {
            // Hitung waktu yang sudah berlalu
            elapsedTime += Time.deltaTime;

            // Lerp posisi x objek dari startXPosition ke targetXPosition
            float newXPosition = Mathf.Lerp(startXPosition, targetXPosition, elapsedTime / duration);
            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
            Debug.Log("Animating... Current position: " + transform.position);

            // Hentikan animasi ketika durasi tercapai
            if (elapsedTime >= duration)
            {
                transform.position = new Vector3(targetXPosition, transform.position.y, transform.position.z); // Pastikan posisi objek tepat di targetXPosition
                isAnimating = false; // Hentikan animasi
                Debug.Log("Animation ended. Final position: " + transform.position);
            }
        }
    }
}
