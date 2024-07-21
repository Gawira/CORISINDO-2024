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

    private void Start()
    {
        if (buttonScript != null)
        {
            buttonScript.OnButtonPressed += ChangeColor; // Berlangganan ke acara tombol ditekan
        }
    }

    private void OnDestroy()
    {
        if (buttonScript != null)
        {
            buttonScript.OnButtonPressed -= ChangeColor; // Berhenti berlangganan dari acara tombol ditekan
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
}
