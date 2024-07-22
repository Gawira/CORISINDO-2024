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

    private void Start()
    {
        if (buttonScript != null)
        {
            buttonScript.OnLeverPulled += ChangeColor; // Subscribe to lever pulled event
            buttonScript.OnButtonPressed += TryOpenDoor; // Subscribe to button pressed event
            Debug.Log("Subscribed to button events.");
        }
        else
        {
            Debug.LogError("buttonScript is not assigned.");
        }

        if (blockRenderer == null)
        {
            Debug.LogError("blockRenderer is not assigned.");
        }

        if (doorScript == null)
        {
            Debug.LogError("doorScript is not assigned.");
        }
    }

    private void OnDestroy()
    {
        if (buttonScript != null)
        {
            buttonScript.OnLeverPulled -= ChangeColor; // Unsubscribe from lever pulled event
            buttonScript.OnButtonPressed -= TryOpenDoor; // Unsubscribe from button pressed event
        }
    }

    private void ChangeColor(AN_Button.LeverType leverType)
    {
        if (blockRenderer != null)
        {
            switch (leverType)
            {
                case AN_Button.LeverType.Accept:
                    blockRenderer.material.color = Color.green;
                    selectedColor = ColorOptions.Hijau;
                    Debug.Log("Button color changed to green.");
                    break;
                case AN_Button.LeverType.Reject:
                    blockRenderer.material.color = Color.red;
                    selectedColor = ColorOptions.Merah;
                    Debug.Log("Button color changed to red.");
                    break;
            }
        }
        else
        {
            Debug.LogError("blockRenderer is not assigned in ChangeColor.");
        }
    }

    private void TryOpenDoor()
    {
        Debug.Log("TryOpenDoor called.");
        if (blockRenderer != null)
        {
            Debug.Log("Button color is: " + blockRenderer.material.color);
            if (blockRenderer.material.color == Color.green && doorScript != null)
            {
                Debug.Log("Opening the door...");
                doorScript.StartMoving();
            }
            else if (blockRenderer.material.color != Color.green)
            {
                Debug.Log("Button color is not green, door will not open.");
            }
            else if (doorScript == null)
            {
                Debug.LogError("doorScript is not assigned.");
            }
        }
        else
        {
            Debug.LogError("blockRenderer is not assigned in TryOpenDoor.");
        }
    }
}
