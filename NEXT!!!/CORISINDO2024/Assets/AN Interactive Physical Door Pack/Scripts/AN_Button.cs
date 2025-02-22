﻿using UnityEngine;
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
    public AudioSource leverPullAudioSource; // AudioSource for the lever pull sound
    public AudioSource stampMachineForwardAudioSource; // AudioSource for the stamp machine forward movement sound
    public AudioSource stampMachineBackwardAudioSource; // AudioSource for the stamp machine backward movement sound

    public delegate void LeverPulledHandler(LeverType leverType);
    public event LeverPulledHandler OnLeverPulled;

    public enum LeverType { Accept, Reject }
    public LeverType leverType;
    public ObjectInteractor objectInteractor;

    Animator anim;
    private static bool isCooldown = false; // Static cooldown flag for both levers

    public GameObject stampSmokeEffectPrefab; // Particle effect prefab for smoke when moving down
    public GameObject stampReturnEffectPrefab; // Particle effect prefab for smoke when moving back
    public Transform smokeEffectSpawnPoint; // The point where the smoke effect will spawn
    public Transform[] returnEffectSpawnPoints; // Array of spawn points for the return effect
    public float afterEffectDuration = 2f; // Duration for the after-effect particle

    public delegate void ButtonPressedHandler();
    public event ButtonPressedHandler OnButtonPressed;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (RampObject != null)
        {
            startYPosition = RampObject.position.y;
            rampQuat = RampObject.rotation;
        }
        startQuat = transform.rotation;

        // Find and assign the ObjectInteractor script if not already assigned in the inspector
        if (objectInteractor == null)
        {
            objectInteractor = FindObjectOfType<ObjectInteractor>();
        }

        // Assign the AudioSource if not assigned in the inspector
        if (leverPullAudioSource == null)
        {
            leverPullAudioSource = GetComponent<AudioSource>();
        }

        if (leverPullAudioSource == null)
        {
            Debug.LogError("Lever Pull AudioSource component is not assigned or found.");
        }

        if (stampMachineForwardAudioSource == null && stampMachine != null)
        {
            stampMachineForwardAudioSource = stampMachine.GetComponent<AudioSource>();
        }

        if (stampMachineForwardAudioSource == null)
        {
            Debug.LogError("Stamp Machine Forward AudioSource component is not assigned or found.");
        }

        if (stampMachineBackwardAudioSource == null && stampMachine != null)
        {
            stampMachineBackwardAudioSource = stampMachine.GetComponent<AudioSource>();
        }

        if (stampMachineBackwardAudioSource == null)
        {
            Debug.LogError("Stamp Machine Backward AudioSource component is not assigned or found.");
        }
    }

    void Update()
    {
        if (!Locked && !isCooldown)
        {
            if (Input.GetMouseButtonDown(0)) // Interaksi tuas dan tombol dengan klik kiri
            {
                if (!DocumentStateManager.IsDocumentAvailable)
                {
                    Debug.Log("Document is not available. Lever cannot be pulled.");
                    return;
                }

                if (!IsPassportOnRightSideTable())
                {
                    Debug.Log("Passport is not on the right side table. Lever cannot be pulled.");
                    return;
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    AN_Button button = hit.collider.GetComponent<AN_Button>();
                    if (button != null && button == this) // Periksa apakah objek yang diklik adalah tuas/tombol ini
                    {
                        if (isLever) // animasi
                        {
                            anim.SetBool("LeverUp", true);
                            HandleLeverPull(); // Tangani logika menarik tuas
                        }
                        else
                        {
                            anim.SetTrigger("ButtonPress");
                            OnButtonPressed?.Invoke(); // Panggil acara tombol ditekan
                        }

                        StartCoroutine(LeverCooldown()); // Mulai cooldown
                    }
                }
            }
        }
    }

    bool IsPassportOnRightSideTable()
    {
        if (objectInteractor != null)
        {
            return objectInteractor.IsPassportOnRightSideTable();
        }
        return false;
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

        OnLeverPulled?.Invoke(leverType); // Call the OnLeverPulled method when the lever is pulled

        // Play the lever pull sound
        if (leverPullAudioSource != null)
        {
            leverPullAudioSource.Play();
        }
    }

    public void TriggerButtonPressed()
    {
        OnButtonPressed?.Invoke();
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

        // Play the stamp machine forward sound
        if (stampMachineForwardAudioSource != null)
        {
            stampMachineForwardAudioSource.Play();
        }

        // Downward movement
        while (elapsedTime < stampSpeed)
        {
            stampMachine.position = Vector3.Lerp(startPosition, stampEndPosition, (elapsedTime / stampSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        stampMachine.position = stampEndPosition;

        // Play the particle effect when the stamp machine reaches the end position
        GameObject smokeEffect = Instantiate(stampSmokeEffectPrefab, smokeEffectSpawnPoint.position, Quaternion.identity);
        ParticleSystem smokeParticles = smokeEffect.GetComponent<ParticleSystem>();
        if (smokeParticles != null)
        {
            smokeParticles.Play();
        }

        yield return new WaitForSeconds(1.4f); // Delay before returning to original position
                                             // Clean up particle effect
        Destroy(smokeEffect);

        elapsedTime = 0f;

        // Play the stamp machine backward sound
        if (stampMachineBackwardAudioSource != null)
        {
            stampMachineBackwardAudioSource.Play();
        }

        // Play the particle effect again when the stamp machine starts returning to the original position
        foreach (Transform spawnPoint in returnEffectSpawnPoints)
        {
            GameObject returnEffect = Instantiate(stampReturnEffectPrefab, spawnPoint.position, Quaternion.identity);
            ParticleSystem returnParticles = returnEffect.GetComponent<ParticleSystem>();
            if (returnParticles != null)
            {
                returnParticles.Play();
            }

            StartCoroutine(DestroyParticleAfterDuration(returnEffect, afterEffectDuration));
        }

        // Upward movement
        while (elapsedTime < stampSpeed)
        {
            stampMachine.position = Vector3.Lerp(stampEndPosition, startPosition, (elapsedTime / stampSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        stampMachine.position = startPosition;
    }

    IEnumerator DestroyParticleAfterDuration(GameObject particleEffect, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(particleEffect);
    }

    IEnumerator LeverCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(999f); // 999 seconds delay till the next target spawn
        isCooldown = false;
    }

    public void ResetLever()
    {
        isCooldown = false;
        // Add any additional reset logic needed for the lever
        Debug.Log("Lever reset.");
    }
}
