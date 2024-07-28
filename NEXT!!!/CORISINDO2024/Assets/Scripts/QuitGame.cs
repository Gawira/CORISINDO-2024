using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quit Game called."); // Debug log for testing
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor, so if we are running in the editor we use this line instead.
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
