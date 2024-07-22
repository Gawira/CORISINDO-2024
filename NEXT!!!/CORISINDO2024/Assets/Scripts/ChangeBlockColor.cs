using UnityEngine;

public class ChangeBlockColor : MonoBehaviour
{
    public enum ColorOptions
    {
        Merah,
        Hijau
    }

    public AN_Button buttonScript; // Referensi ke skrip tombol
    public Renderer blockRenderer; // Referensi ke renderer blok
    public ColorOptions selectedColor = ColorOptions.Hijau; // Warna yang dipilih
    public MoveObject doorScript; // Reference to the MoveObject script controlling the door

    private void Start()
    {
        if (buttonScript != null)
        {
            buttonScript.OnButtonPressed += ChangeColor; // Berlangganan ke acara tombol ditekan
            buttonScript.OnButtonPressed += TryOpenDoor; // Subscribe to button press event to try opening the door
        }
    }

    private void OnDestroy()
    {
        if (buttonScript != null)
        {
            buttonScript.OnButtonPressed -= ChangeColor; // Berhenti berlangganan dari acara tombol ditekan
            buttonScript.OnButtonPressed -= TryOpenDoor; // Unsubscribe from button press event
        }
    }

    private void ChangeColor()
    {
        if (blockRenderer != null)
        {
            switch (selectedColor)
            {
                case ColorOptions.Hijau:
                    blockRenderer.material.color = Color.green;
                    break;
                case ColorOptions.Merah:
                    blockRenderer.material.color = Color.red;
                    break;
            }
        }
    }

    private void TryOpenDoor()
    {
        if (blockRenderer != null && blockRenderer.material.color == Color.green && doorScript != null)
        {
            doorScript.StartMoving();
        }
    }
}
