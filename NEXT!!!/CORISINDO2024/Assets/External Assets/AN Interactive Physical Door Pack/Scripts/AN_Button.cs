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

    Animator anim;
    private static bool isGlobalCooldown = false; // Static variable for global cooldown
    public float cooldownDuration = 5f; // Cooldown duration

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
        if (!Locked && !isGlobalCooldown)
        {
            if (Input.GetMouseButtonDown(0)) // Lever or button interaction with left-click
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    AN_Button button = hit.collider.GetComponent<AN_Button>();
                    if (button != null && button == this) // Check if the clicked object is this lever/button
                    {
                        Debug.Log("Lever clicked: " + hit.collider.gameObject.name);
                        if (isLever) // animations
                        {
                            anim.SetBool("LeverUp", true);
                            StartCoroutine(LeverCooldown());
                        }
                        else
                        {
                            anim.SetTrigger("ButtonPress");
                            StartCoroutine(LeverCooldown());
                        }
                    }
                }
            }

            // Uncomment and adjust the following block if you need the valve behavior
            /*
            else if (isValve && RampObject != null) // Valve
            {
                if (Input.GetMouseButton(0) && NearView())
                {
                    if (valveBool)
                    {
                        if (!isOpened && CanOpen && current < max) current += speed * Time.deltaTime;
                        if (isOpened && CanClose && current > min) current -= speed * Time.deltaTime;

                        if (current >= max)
                        {
                            isOpened = true;
                            valveBool = false;
                        }
                        else if (current <= min)
                        {
                            isOpened = false;
                            valveBool = false;
                        }
                    }
                }
                else
                {
                    if (!isOpened && current > min) current -= speed * Time.deltaTime;
                    if (isOpened && current < max) current += speed * Time.deltaTime;
                    valveBool = true;
                }

                transform.rotation = startQuat * Quaternion.Euler(0f, 0f, current * ValveSpeed);
                if (xRotation) RampObject.rotation = rampQuat * Quaternion.Euler(current, 0f, 0f);
                else if (yPosition) RampObject.position = new Vector3(RampObject.position.x, startYPosition + current, RampObject.position.z);
            }
            */
        }
    }

    IEnumerator LeverCooldown()
    {
        isGlobalCooldown = true; // Start global cooldown
        yield return new WaitForSeconds(cooldownDuration); // Wait for cooldown duration
        isGlobalCooldown = false; // End global cooldown
    }
}
