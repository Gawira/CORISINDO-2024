using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class SimpleButton : MonoBehaviour
{
    public GameValues gameValues;
    public Renderer blockRenderer;
    public MoveObject doorScript;
    public Color greenColor = Color.green;
    public Color redColor = Color.red;
    public Color defaultColor = Color.white; // Default color for the button when reset
    public float doorOpenDelay = 5.0f; // Delay before button can be pressed again
    public BotTeleporter botTeleporter; // Reference to the BotTeleporter script
    public float documentMoveDuration = 1.0f; // Duration to move document
    public float documentLiftHeight = 0.15f; // Height to lift the document

    public GameObject[] darahBiruIndicators; // Array to hold Darah biru indicators
    public GameObject peringatan; // Reference to the Peringatan game object
    private int mistakesCount = 0; // Counter for the number of mistakes

    public event Action OnButtonPressed;

    private bool isCooldown = false;
    private bool isPunished = false; // Flag to ensure punishment is only applied once per robot

    private VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    public bool hasPunishedUser = false; // Flag to ensure the user is punished only once per robot
    public bool initialDecisionCorrect = false; // Flag to track if the initial decision was correct
    public AudioSource buttonPressAudioSource; // AudioSource for button press sound

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

        if (peringatan == null)
        {
            Debug.LogError("Peringatan is not assigned.");
        }

        if (darahBiruIndicators == null || darahBiruIndicators.Length == 0)
        {
            Debug.LogError("Darah Biru Indicators are not assigned.");
        }

        // Ensure the Peringatan object has a VideoPlayer component
        if (peringatan != null)
        {
            videoPlayer = peringatan.GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                Debug.LogError("VideoPlayer component is not assigned to Peringatan game object.");
            }
        }

        gameValues = GameValues.Instance; // Ensure gameValues is correctly assigned

        // Ensure the AudioSource is assigned
        if (buttonPressAudioSource == null)
        {
            buttonPressAudioSource = GetComponent<AudioSource>();
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

        GameObject teleportedBot = botTeleporter.GetTeleportedBot();
        if (teleportedBot != null)
        {
            RobotController robotController = teleportedBot.GetComponent<RobotController>();
            if (robotController != null)
            {
                robotController.ButtonPressed(); // Notify the RobotController that the button has been pressed

                string robotCategory = robotController.GetCategory();
                bool isMistake = false;

                if ((blockRenderer.material.color == greenColor && robotCategory == "Rejected") ||
                    (blockRenderer.material.color == redColor && robotCategory == "Accepted"))
                {
                    // User made a mistake
                    isMistake = true;
                    initialDecisionCorrect = false;
                }
                else
                {
                    initialDecisionCorrect = true;
                }

                if (isMistake && !hasPunishedUser)
                {
                    // Apply punishment if not already punished
                    hasPunishedUser = true; // Mark that punishment has been applied
                    StartCoroutine(HandleMistake());
                    gameValues.AddMistake(); // Add mistake to GameValues
                }
                else
                {
                    gameValues.AddMoney(100000);
                    gameValues.AddCorrectDecision();
                    Debug.Log("Money Added. New Balance: " + gameValues.GetMoney());
                }

                // Move the robot regardless of whether the user made a mistake
                if (blockRenderer.material.color == greenColor)
                {
                    TryOpenDoor();
                    MoveDocuments();
                    StartCoroutine(StartAcceptedBotAnimation());
                }
                else if (blockRenderer.material.color == redColor)
                {
                    MoveDocuments();
                    StartCoroutine(StartRejectedBotAnimation());
                }

                StartCoroutine(CooldownCoroutine());

                // Play the button press sound
                if (buttonPressAudioSource != null)
                {
                    buttonPressAudioSource.Play();
                }
            }
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

                Debug.Log("Robot destroyed");
                GameValues.Instance.RobotDestroyed(); // Notify that the robot has been destroyed

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
                if (UnityEngine.Random.value < 1f) // 20% chance to trigger yelling
                {
                    robotController.TriggerYelling();
                    yield return new WaitUntil(() => robotController.HitCount >= 3);
                    robotController.StopYelling();
                }

                yield return robotController.RotateToAngle(90f);
                robotController.animator.SetBool("Walk", true);

                Vector3 targetPosition = robotController.transform.position + new Vector3(0, 0, -5f);
                yield return robotController.MoveToPosition(targetPosition);

                robotController.animator.SetBool("Walk", false);
                Destroy(robotController.gameObject); // Destroy the bot after it reaches the target position

                Debug.Log("Robot destroyed");
                GameValues.Instance.RobotDestroyed(); // Notify that the robot has been destroyed

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

    public IEnumerator MoveDocumentCoroutine(Transform document, Rigidbody rb)
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

        // Reset the punishment flag for the next robot
        isPunished = false;
    }

    public void SetColor(Color color)
    {
        if (blockRenderer != null)
        {
            blockRenderer.material.color = color;
            Debug.Log("Button color changed to: " + color);
        }
    }

    public void ResetButtonAndLever()
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

    public IEnumerator HandleMistake()
    {
        gameValues.SubtractMoney(50000);
        Debug.Log("Money Subtracted. New Balance: " + gameValues.GetMoney());
        Debug.Log("Starting punishment video...");
        peringatan.SetActive(true);

        // Ensure the VideoPlayer component is assigned
        if (videoPlayer == null)
        {
            videoPlayer = peringatan.GetComponent<VideoPlayer>();
        }

        // Wait until the video has finished playing
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
        }
        else
        {
            // Fallback delay if no VideoPlayer component is found
            yield return new WaitForSecondsRealtime(4.55f);
        }

        Debug.Log("Stopping punishment video...");
        peringatan.SetActive(false);

        // Disable one "Darah biru" indicator
        if (mistakesCount < darahBiruIndicators.Length)
        {
            darahBiruIndicators[mistakesCount].SetActive(false);
            mistakesCount++;

            // Check if the user has run out of lives
            if (mistakesCount >= darahBiruIndicators.Length)
            {
                // Handle game over logic here
                Debug.Log("Game Over");
                if (gameValues != null)
                {
                    gameValues.ChangeScene(); // Transition to the Day transition scene
                }
            }
        }
    }
}
