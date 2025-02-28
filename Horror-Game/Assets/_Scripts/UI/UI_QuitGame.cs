using UnityEngine;
using UnityEngine.UI;

public class UI_QuitGame : MonoBehaviour
{
    [SerializeField] private Button quitButton; // Reference to the UI Button

    void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(Quit); // Add event listener in script
        }
        else
        {
            Debug.LogError("Quit Button is not assigned in the Inspector!");
        }
    }

    void Quit()
    {
        Debug.Log("Game is closing...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in Unity
#else
            Application.Quit(); // Close the application in a build
#endif
    }
}