using UnityEngine;

public class ChangeBlockColor : MonoBehaviour
{
    public enum ColorOptions
    {
        Merah,
        Hijau
    }

    public AN_Button buttonScript; // Reference to the button script
    public Renderer blockRenderer; // Reference to the block renderer
    public ColorOptions selectedColor = ColorOptions.Hijau; // Selected color
    public MoveObject doorScript; // Reference to the MoveObject script controlling the door
    public CameraSwitcher cameraSwitcher; // Reference to the CameraSwitcher script

    private void Start()
    {
        if (buttonScript != null)
        {
            buttonScript.OnButtonPressed += ChangeColor; // Subscribe to the button pressed event to change color
            buttonScript.OnButtonPressed += TryOpenDoor; // Subscribe to the button pressed event to try opening the door
        }
        else
        {
            Debug.LogError("ButtonScript is not assigned.");
        }

        if (blockRenderer == null)
        {
            blockRenderer = GetComponent<Renderer>();
            if (blockRenderer == null)
            {
                Debug.LogError("BlockRenderer is not assigned and not found on the same GameObject.");
            }
        }

        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindObjectOfType<CameraSwitcher>();
            if (cameraSwitcher == null)
            {
                Debug.LogError("CameraSwitcher is not assigned and not found in the scene.");
            }
        }

        if (doorScript == null)
        {
            doorScript = FindObjectOfType<MoveObject>();
            if (doorScript == null)
            {
                Debug.LogError("DoorScript is not assigned and not found in the scene.");
            }
        }
    }

    private void OnDestroy()
    {
        if (buttonScript != null)
        {
            buttonScript.OnButtonPressed -= ChangeColor; // Unsubscribe from the button pressed event
            buttonScript.OnButtonPressed -= TryOpenDoor; // Unsubscribe from the button pressed event
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
            Debug.Log("Block color changed to: " + selectedColor); // Add this debug log
        }
    }

    private void TryOpenDoor()
    {
        Debug.Log("TryOpenDoor called."); // Add this debug log
        if (blockRenderer != null && blockRenderer.material.color == Color.green && doorScript != null && cameraSwitcher != null)
        {
            if (!cameraSwitcher.IsInTopDownView() && !cameraSwitcher.IsTransitioning())
            {
                doorScript.StartMoving();
                Debug.Log("Door opening triggered by button press."); // Add this debug log
            }
            else
            {
                Debug.Log("Button press ignored. Camera not in main view or transitioning."); // Add this debug log
            }
        }
        else
        {
            Debug.Log("Button press ignored. Block color is not green or doorScript is null."); // Add this debug log
        }
    }

    public void SetColor(ColorOptions color)
    {
        selectedColor = color;
        ChangeColor();
    }
}
