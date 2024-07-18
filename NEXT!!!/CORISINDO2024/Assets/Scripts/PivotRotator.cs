using UnityEngine;

public class PivotRotator : MonoBehaviour
{
    public Transform pivot; // Titik poros
    public float rotationSpeed = 100f; // Kecepatan rotasi dalam derajat per detik

    void Update()
    {
        // Rotasi objek di sekitar titik poros
        transform.RotateAround(pivot.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
