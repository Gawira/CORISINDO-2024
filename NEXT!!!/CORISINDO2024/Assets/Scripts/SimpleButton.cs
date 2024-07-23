using UnityEngine;
using System.Collections;
using System;

public class SimpleButton : MonoBehaviour
{
    public Renderer blockRenderer;
    public MoveObject doorScript;
    public Color greenColor = Color.green;
    public Color redColor = Color.red;
    public Color defaultColor = Color.white; // Default color for the button when reset
    public float doorOpenDelay = 5.0f; // Delay before button can be pressed again
    public BotTeleporter botTeleporter; // Reference to the BotTeleporter script
    public float documentMoveDuration = 1.0f; // Duration to move document
    public float documentLiftHeight = 0.15f; // Height to lift the document

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

        if (botTeleporter == null)
        {
            Debug.LogError("Bot Teleporter is not assigned.");
        }
    }

    public void PressButton()
    {
        if (isCooldown || !DocumentStateManager.IsDocumentAvailable)
        {
            Debug.Log("Button is in cooldown or document is not available, please wait.");
            return;
        }

        Debug.Log("Button Pressed!");
        OnButtonPressed?.Invoke();

        if (blockRenderer.material.color == greenColor)
        {
            TryOpenDoor();
            MoveDocuments();
            StartCoroutine(StartAcceptedBotAnimation());
            StartCoroutine(CooldownCoroutine());
        }
        else if (blockRenderer.material.color == redColor)
        {
            MoveDocuments();
            StartCoroutine(StartRejectedBotAnimation());
            StartCoroutine(CooldownCoroutine());
        }
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

    private IEnumerator StartAcceptedBotAnimation()
    {
        GameObject teleportedBot = botTeleporter.GetTeleportedBot();
        if (teleportedBot != null)
        {
            RobotController robotController = teleportedBot.GetComponent<RobotController>();
            if (robotController != null)
            {
                yield return robotController.RotateToAngle(-90f);

                robotController.animator.SetBool("Walk", true);

                Vector3 targetPosition = robotController.transform.position + new Vector3(0, 0, 5f);
                yield return robotController.MoveToPosition(targetPosition);

                robotController.animator.SetBool("Walk", false);
                Destroy(robotController.gameObject); // Destroy the bot after it reaches the target position

                // Spawn a new bot after the current one is destroyed
                botTeleporter.SpawnNewBot();

                // Reset button and lever states
                ResetButtonAndLever();
            }
        }
    }

    private IEnumerator StartRejectedBotAnimation()
    {
        GameObject teleportedBot = botTeleporter.GetTeleportedBot();
        if (teleportedBot != null)
        {
            RobotController robotController = teleportedBot.GetComponent<RobotController>();
            if (robotController != null)
            {
                yield return robotController.RotateToAngle(90f);

                robotController.animator.SetBool("Walk", true);

                Vector3 targetPosition = robotController.transform.position + new Vector3(0, 0, -5f);
                yield return robotController.MoveToPosition(targetPosition);

                robotController.animator.SetBool("Walk", false);
                Destroy(robotController.gameObject); // Destroy the bot after it reaches the target position

                // Spawn a new bot after the current one is destroyed
                botTeleporter.SpawnNewBot();

                // Reset button and lever states
                ResetButtonAndLever();
            }
        }
    }

    private void MoveDocuments()
    {
        GameObject[] documents = GameObject.FindGameObjectsWithTag("Document");
        foreach (GameObject doc in documents)
        {
            Rigidbody rb = doc.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            StartCoroutine(MoveDocumentCoroutine(doc.transform, rb));
        }
    }

    private IEnumerator MoveDocumentCoroutine(Transform document, Rigidbody rb)
    {
        Vector3 startPosition = document.position;
        Vector3 intermediatePosition = new Vector3(startPosition.x, startPosition.y + documentLiftHeight, startPosition.z);
        Vector3 endPosition = new Vector3(document.position.x - 2f, document.position.y, document.position.z);
        float elapsedTime = 0;

        // Lift document up
        while (elapsedTime < documentMoveDuration / 2)
        {
            document.position = Vector3.Lerp(startPosition, intermediatePosition, elapsedTime / (documentMoveDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        document.position = intermediatePosition;

        // Move document to end position
        elapsedTime = 0;
        while (elapsedTime < documentMoveDuration / 2)
        {
            document.position = Vector3.Lerp(intermediatePosition, endPosition, elapsedTime / (documentMoveDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        document.position = endPosition;

        yield return new WaitForSeconds(0.5f); // Optional delay before moving back

        Debug.Log("Document moved back to spawn position. Destroying document.");
        Destroy(document.gameObject); // Destroy the document after reaching the spawn position

        if (rb != null)
        {
            rb.isKinematic = false;
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

    private void ResetButtonAndLever()
    {
        // Reset button cooldown
        isCooldown = false;
        // Reset button color to default
        blockRenderer.material.color = defaultColor;

        // Reset lever state
        AN_Button leverScript = FindObjectOfType<AN_Button>(); // Find the lever script instance
        if (leverScript != null)
        {
            leverScript.ResetLever();
        }

        // Reset document availability state
        DocumentStateManager.IsDocumentAvailable = false;
    }
}
