using UnityEngine;

public class ObjectInteractor : MonoBehaviour
{
    public CameraSwitcher cameraSwitcher;
    public Camera playerCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!cameraSwitcher.IsInTopDownView())
            {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        InteractWithObject(hit.collider.gameObject);
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1)) // Detect right-click
        {
            if (cameraSwitcher.CanRightClick())
            {
                Debug.Log("Right-click detected, switching to main view...");
                cameraSwitcher.SwitchToMainView();
            }
        }
    }

    void InteractWithObject(GameObject obj)
    {
        // For now, we'll assume any click on the table switches the view
        if (obj.CompareTag("Workstation"))
        {
            Debug.Log("Interacting with workstation.");
            cameraSwitcher.SwitchToTopDownView();
        }
    }
}
