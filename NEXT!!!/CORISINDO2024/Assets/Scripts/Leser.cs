using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour
{
    public float endXPosition;   // Posisi akhir x objek
    public float duration = 5.0f; // Durasi perpindahan
    public float returnDuration = 5.0f; // Durasi sebelum kembali ke posisi semula

    private float startXPosition;
    private float elapsedTime = 0;
    private bool isAnimating = false;
    private bool isReturning = false; // Menyimpan status apakah objek sedang kembali ke posisi semula

    void Start()
    {
        // Inisialisasi posisi awal objek
        startXPosition = transform.position.x;
        Debug.Log("Posisi x awal: " + startXPosition);
    }

    void Update()
    {
        // Jika animasi sedang berjalan
        if (isAnimating)
        {
            // Hitung waktu yang sudah berlalu
            elapsedTime += Time.deltaTime;

            // Lerp posisi x objek dari startXPosition ke endXPosition
            float newXPosition = Mathf.Lerp(startXPosition, endXPosition, elapsedTime / duration);
            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
            Debug.Log("Animating... Posisi saat ini: " + transform.position);

            // Hentikan animasi ketika durasi tercapai
            if (elapsedTime >= duration)
            {
                transform.position = new Vector3(endXPosition, transform.position.y, transform.position.z); // Pastikan posisi objek tepat di endXPosition
                isAnimating = false; // Hentikan animasi
                Debug.Log("Animasi berakhir. Posisi akhir: " + transform.position);

                // Mulai kembalikan objek ke posisi semula setelah durasi tertentu
                StartCoroutine(KembaliKePosisiSemula());
            }
        }
    }

    public void StartMoving()
    {
        if (!isAnimating && !isReturning)
        {
            isAnimating = true;
            elapsedTime = 0; // Reset waktu
            startXPosition = transform.position.x; // Set posisi x awal ketika animasi dimulai
            Debug.Log("Animasi dimulai. Posisi x awal: " + startXPosition + " Posisi x akhir: " + endXPosition);
        }
    }

    private IEnumerator KembaliKePosisiSemula()
    {
        isReturning = true;
        float returnElapsedTime = 0;
        float returnStartXPosition = transform.position.x;

        // Tunggu durasi sebelum kembali ke posisi semula
        yield return new WaitForSeconds(returnDuration);

        while (returnElapsedTime < duration)
        {
            returnElapsedTime += Time.deltaTime;
            float newXPosition = Mathf.Lerp(returnStartXPosition, startXPosition, returnElapsedTime / duration);
            transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
            Debug.Log("Kembali... Posisi saat ini: " + transform.position);
            yield return null;
        }

        transform.position = new Vector3(startXPosition, transform.position.y, transform.position.z); // Pastikan posisi objek tepat di startXPosition
        isReturning = false; // Hentikan pengembalian
        Debug.Log("Kembali berakhir. Posisi akhir: " + transform.position);
    }
}
