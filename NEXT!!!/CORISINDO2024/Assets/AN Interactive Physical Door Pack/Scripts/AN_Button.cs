using UnityEngine;
using System.Collections;

public class AN_Button : MonoBehaviour
{
    [Tooltip("True for rotation like valve (used for ramp/elevator only)")]
    public bool isValve = false;
    [Tooltip("SelfRotation speed of valve")]
    public float ValveSpeed = 10f;
    [Tooltip("If it isn't valve, it can be lever or button (animated)")]
    public bool isLever = false;
    [Tooltip("If it is false door can't be used")]
    public bool Locked = false;
    [Tooltip("The door for remote control")]
    public AN_DoorScript DoorObject;
    [Space]
    [Tooltip("Any object for ramp/elevator behaviour")]
    public Transform RampObject;
    [Tooltip("Door can be opened")]
    public bool CanOpen = true;
    [Tooltip("Door can be closed")]
    public bool CanClose = true;
    [Tooltip("Current status of the door")]
    public bool isOpened = false;
    [Space]
    [Tooltip("True for rotation by X local rotation by valve")]
    public bool xRotation = true;
    [Tooltip("True for vertical movement by valve (if xRotation is false)")]
    public bool yPosition = false;
    public float max = 90f, min = 0f, speed = 5f;
    bool valveBool = true;
    float current, startYPosition;
    Quaternion startQuat, rampQuat;

    public Light pointLight; // Reference to the light object
    public Color greenLightColor = Color.green;
    public Color redLightColor = Color.red;
    public Transform stampMachine; // Reference to the STAMP MACHINE
    public Vector3 stampEndPosition; // End position for the STAMP MACHINE
    public float stampSpeed = 0.1f; // Speed of the STAMP MACHINE movement

    Animator anim;
    private static bool isCooldown = false; // Static cooldown flag for both levers

    void Start()
    {
        anim = GetComponent<Animator>();
        if (RampObject != null)
        {
            startYPosition = RampObject.position.y;
            rampQuat = RampObject.rotation;
        }
        startQuat = transform.rotation;
    }

    void Update()
    {
        if (!Locked && !isCooldown)
        {
            if (Input.GetMouseButtonDown(0)) // Lever and button interaction with left-click
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    AN_Button button = hit.collider.GetComponent<AN_Button>();
                    if (button != null && button == this) // Check if the clicked object is this lever/button
                    {
                        if (isLever) // animations
                        {
                            anim.SetBool("LeverUp", true);
                            HandleLeverPull(); // Handle lever pull logic
                        }
                        else
                        {
                            anim.SetTrigger("ButtonPress");
                        }

                        StartCoroutine(LeverCooldown()); // Start the cooldown
                    }
                }
            }
        }
    }

    void HandleLeverPull()
    {
        if (pointLight != null)
        {
            if (gameObject.name.Contains("Green")) // Assuming the lever names contain "Green" or "Red"
            {
                StartCoroutine(ChangeLightColor(greenLightColor));
            }
            else if (gameObject.name.Contains("Red"))
            {
                StartCoroutine(ChangeLightColor(redLightColor));
            }
        }

        if (stampMachine != null)
        {
            StartCoroutine(MoveStampMachine());
        }
    }

    IEnumerator ChangeLightColor(Color newColor)
    {
        Color originalColor = pointLight.color;
        pointLight.color = newColor;
        yield return new WaitForSeconds(2.5f); // Light color duration
        pointLight.color = originalColor;
    }

    IEnumerator MoveStampMachine()
    {
        Vector3 startPosition = stampMachine.position;
        float elapsedTime = 0f;
        while (elapsedTime < stampSpeed)
        {
            stampMachine.position = Vector3.Lerp(startPosition, stampEndPosition, (elapsedTime / stampSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        stampMachine.position = stampEndPosition;

        yield return new WaitForSeconds(1.5f); // Delay before returning to original position

        elapsedTime = 0f;
        while (elapsedTime < stampSpeed)
        {
            stampMachine.position = Vector3.Lerp(stampEndPosition, startPosition, (elapsedTime / stampSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        stampMachine.position = startPosition;
    }

    IEnumerator LeverCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(5f); // 5 seconds delay
        isCooldown = false;
    }
}
