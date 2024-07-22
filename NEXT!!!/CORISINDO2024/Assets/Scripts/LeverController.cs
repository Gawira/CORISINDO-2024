using UnityEngine;

public class LeverController : MonoBehaviour
{
    public enum LeverType { Accept, Reject }
    public LeverType leverType;
    public ObjectInteractor objectInteractor;

    // Call this method when the lever is pulled
    public void OnLeverPulled()
    {
        Debug.Log("Lever pulled: " + leverType); // Add this debug log to check if the method is called
        if (leverType == LeverType.Accept)
        {
            Debug.Log("Accept lever pulled");
            objectInteractor.LabelPassport("Accepted");
        }
        else if (leverType == LeverType.Reject)
        {
            Debug.Log("Reject lever pulled");
            objectInteractor.LabelPassport("Rejected");
        }
    }
}
