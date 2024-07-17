using UnityEngine;

public class OrbitAndRotate : MonoBehaviour
{
    public float rotationSpeed = 100f; // Kecepatan rotasi dalam derajat per detik
    public float orbitSpeed = 50f; // Kecepatan orbit dalam derajat per detik
    public float radius = 2f; // Radius orbit

    private float angle = 0f; // Sudut orbit

    void Update()
    {
        // Update sudut orbit
        angle += orbitSpeed * Time.deltaTime;

        // Hitung posisi baru
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        transform.position = new Vector3(x, transform.position.y, z);

        // Rotasi objek setiap frame pada sumbu Y
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
