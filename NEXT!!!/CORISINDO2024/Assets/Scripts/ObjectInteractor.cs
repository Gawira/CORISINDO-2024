using UnityEngine;

public class ObjectInteractor : MonoBehaviour
{
    public CameraSwitcher cameraSwitcher;
    public Camera playerCamera;

    void Update()
    {
        if (!cameraSwitcher.CanUseInput())
        {
            return; // Disable all input during transition
        }

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

        if (cameraSwitcher.CanUseInput() && !cameraSwitcher.IsInTopDownView())
        {
            // Handle A and D key presses for camera rotation
            if (Input.GetKeyDown(KeyCode.A))
            {
                // Rotate camera left
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                // Rotate camera right
            }
        }
    }

    void InteractWithObject(GameObject obj)
    {
        if (!cameraSwitcher.CanUseInput())
        {
            return; // Disable all input during transition
        }

        if (obj.CompareTag("Workstation"))
        {
            Debug.Log("Interacting with workstation.");
            cameraSwitcher.SwitchToTopDownView();
        }
    }
}
