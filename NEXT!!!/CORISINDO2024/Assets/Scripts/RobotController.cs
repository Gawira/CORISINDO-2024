using System.Collections;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public Animator animator;
    public Transform targetPoint;
    public float walkSpeed = 2f;
    private bool isWalking = false;

    private int robotID;
    private string category;

    public delegate void DocumentGiveHandler();
    public event DocumentGiveHandler OnDocumentGive;

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
        if (other.CompareTag("Bat"))
        {
            GotHit();
        }
    }

    public void GotHit()
    {
        // Trigger the "Taking Hit" animation
        animator.SetBool("Taking Hit", true);
        StartCoroutine(ResetTakingHit());
    }

    private IEnumerator ResetTakingHit()
    {
        yield return new WaitForSeconds(1f); // Adjust the duration to match the "Taking Hit" animation length
        animator.SetBool("Taking Hit", false);
    }
}
