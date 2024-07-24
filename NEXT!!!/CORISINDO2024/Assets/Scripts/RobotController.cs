using System.Collections;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public Animator animator;
    public Transform targetPoint;
    public float walkSpeed = 2f;
    private bool isWalking = false;
    private int hitCount = 0; // Counter for the number of hits
    private int robotID;
    private string category;
    private bool canTakeHit = false; // Flag to check if the bot can take a hit

    public delegate void DocumentGiveHandler();
    public event DocumentGiveHandler OnDocumentGive;

    public BotTeleporter botTeleporter; // Reference to the BotTeleporter script
    public SimpleButton simpleButton; // Reference to the SimpleButton script

    private void Start()
    {
        StartCoroutine(PerformActions());
    }

    public void SetID(int id, string category)
    {
        this.robotID = id;
        this.category = category;
        // Optionally, update the robot's appearance or UI with the ID and category
        Debug.Log($"Robot {category} ID: {id}");
    }

    public int GetID()
    {
        return robotID;
    }

    public string GetCategory()
    {
        return category;
    }

    private IEnumerator PerformActions()
    {
        // Start walking
        animator.SetBool("Walk", true);
        isWalking = true;

        // Move to the target point on the Z axis
        yield return MoveToPosition(new Vector3(targetPoint.position.x, transform.position.y, targetPoint.position.z));

        // Trigger the LookRight animation and rotate 90 degrees to the right
        animator.SetBool("Walk", false);
        yield return RotateToAngle(90f);

        // Trigger the giving animation
        animator.SetTrigger("Giving");

        // Wait for the giving animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Trigger the document give event
        OnDocumentGive?.Invoke();

        // Allow the bot to take hits
        canTakeHit = true;

        // Switch to idle animation
        animator.Play("Idle");

    }

    public IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, walkSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator RotateToAngle(float angle)
    {
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, walkSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bat") && canTakeHit)
        {
            GotHit();
        }
    }

    public void GotHit()
    {
        // Check if the hit count is less than 3 before registering the hit
        if (hitCount >= 3)
        {
            return; // Do not register the hit if the count is 3 or more
        }

        // Increment the hit counter
        hitCount++;

        // Force transition to idle before restarting "Taking Hit" animation
        animator.Play("Idle", 0, 0);
        animator.Play("Taking Hit", 0, 0);

        // If the bot has been hit 3 times, move it to the left side
        if (hitCount == 3)
        {
            // Move documents back
            MoveDocumentsBack();
            StartCoroutine(simpleButton.HandleMistake());
            StartCoroutine(MoveToLeftSide());
        }
    }

    private IEnumerator MoveToLeftSide()
    {
        // Wait for the "Taking Hit" animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        yield return RotateToAngle(90f); // Rotate to face left side
        animator.SetBool("Walk", true);
        Vector3 targetPosition = transform.position + new Vector3(0, 0, -5f); // Move towards the left side
        yield return MoveToPosition(targetPosition);
        animator.SetBool("Walk", false);

        Destroy(gameObject); // Optionally destroy the bot after it moves to the left side

        // Spawn a new bot after the current one is destroyed
        botTeleporter.SpawnNewBot();

        // Reset button and lever states
        simpleButton.ResetButtonAndLever();
    }

    private void MoveDocumentsBack()
    {
        GameObject[] documents = GameObject.FindGameObjectsWithTag("Document");
        foreach (GameObject doc in documents)
        {
            Rigidbody rb = doc.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            StartCoroutine(simpleButton.MoveDocumentCoroutine(doc.transform, rb));
        }
    }
}
