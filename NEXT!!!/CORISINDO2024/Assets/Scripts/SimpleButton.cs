using UnityEngine;
using System.Collections;
using System;

public class SimpleButton : MonoBehaviour
{
    public Renderer blockRenderer;
    public MoveObject doorScript;
    public Color greenColor = Color.green;
    public Color redColor = Color.red;
    public float doorOpenDelay = 5.0f; // Delay before button can be pressed again

    public event Action OnButtonPressed;

    private bool isCooldown = false;

    private void Start()
    {
        if (blockRenderer == null)
        {
            Debug.LogError("Block Renderer is not assigned.");
        }

        if (doorScript == null)
        {
            Debug.LogError("Door Script is not assigned.");
        }
    }

    public void PressButton()
    {
        if (isCooldown)
        {
            Debug.Log("Button is in cooldown, please wait.");
            return;
        }

        Debug.Log("Button Pressed!");
        OnButtonPressed?.Invoke();
        TryOpenDoor();
        StartCoroutine(CooldownCoroutine());
    }

    private void TryOpenDoor()
    {
        Debug.Log("TryOpenDoor called.");
        if (blockRenderer != null)
        {
            Debug.Log("Button color is: " + blockRenderer.material.color);
            if (blockRenderer.material.color == greenColor && doorScript != null)
            {
                Debug.Log("Opening the door...");
                doorScript.StartMoving();
            }
            else if (blockRenderer.material.color != greenColor)
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

    private IEnumerator CooldownCoroutine()
    {
        isCooldown = true;
        Debug.Log("Button is in cooldown.");
        yield return new WaitForSeconds(doorOpenDelay); // Cooldown duration
        isCooldown = false;
        Debug.Log("Button cooldown ended, it can be pressed again.");
    }

    public void SetColor(Color color)
    {
        if (blockRenderer != null)
        {
            blockRenderer.material.color = color;
            Debug.Log("Button color changed to: " + color);
        }
    }
}
