using System.Collections;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public Animator animator;
    public Transform targetPoint;
    public float walkSpeed = 2f;
    private int hitCount = 0; // Counter for the number of hits
    private int robotID;
    private string category;
    private bool canTakeHit = false; // Flag to check if the bot can take a hit
    private bool isYelling = false; // Flag for yelling state
    private bool hasBeenPunished = false; // Flag to check if the user has already been punished
    private bool isHitBackwardsFinished = false; // Flag to indicate if the "Hit Backwards" animation has finished

    public delegate void DocumentGiveHandler();
    public event DocumentGiveHandler OnDocumentGive;

    public BotTeleporter botTeleporter; // Reference to the BotTeleporter script
    public SimpleButton simpleButton; // Reference to the SimpleButton script

    private ObjectInteractor objectInteractor; // Reference to ObjectInteractor script

    public int HitCount
    {
        get { return hitCount; }
    }

    private void Start()
    {
        GameValues.Instance.RobotSpawned(); // Notify that a robot has spawned
        StartCoroutine(PerformActions());

        objectInteractor = FindObjectOfType<ObjectInteractor>(); // Find and assign ObjectInteractor reference
    }

    public void SetID(int id, string category)
    {
        this.robotID = id;
        this.category = category;
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
        simpleButton.hasPunishedUser = false;

        // Start walking
        animator.SetBool("Walk", true);

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
        animator.SetBool("Walk", false);
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
        if (hitCount >= 3)
        {
            return; // Do not register the hit if the count is 3 or more
        }

        hitCount++;

        objectInteractor.PlayHitSound();

        animator.Play("Idle", 0, 0);
        animator.Play("Taking Hit", 0, 0);

        if (hitCount == 3)
        {
            if (isYelling && simpleButton.initialDecisionCorrect)
            {
                MoveDocumentsBack();
                StartCoroutine(PlayHitBackwardsAnimation());
            }
            else
            {
                StartCoroutine(PlayHitBackwardsAnimation());
                MoveDocumentsBack();

                if (!simpleButton.hasPunishedUser)
                {
                    simpleButton.hasPunishedUser = true;
                    StartCoroutine(simpleButton.HandleMistake());
                }
            }
        }
    }

    private IEnumerator PlayHitBackwardsAnimation()
    {
        animator.Play("Hit Backwards", 0, 0);
        isHitBackwardsFinished = true;

        yield return new WaitForSeconds(4.6f);

        if (this != null)
        {
            Destroy(gameObject);

            if (simpleButton != null)
            {
                simpleButton.ResetButtonAndLever();
            }
        }

        GameValues.Instance.RobotDestroyed();

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

    public void TriggerYelling()
    {
        isYelling = true;
        animator.SetBool("Yell", true);
        StartCoroutine(YellingLoop());
    }

    public void StopYelling()
    {
        isYelling = false;
        animator.SetBool("Yell", false);
    }

    private IEnumerator YellingLoop()
    {
        while (isYelling)
        {
            animator.Play("Yell", 0, 0);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    public void SetPunished()
    {
        hasBeenPunished = true;
    }

    public void OnHitBackwardsFinished()
    {
        isHitBackwardsFinished = true;
    }

    public void ButtonPressed()
    {
        simpleButton.hasPunishedUser = false;
    }
}
