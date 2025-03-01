using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class UI_PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // Assign the UI Panel in Inspector
    [SerializeField] private Button quitButton; // Assign the Quit Button in Inspector
    private bool isPaused = false;

    void Start()
    {
        // Ensure menu starts hidden
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        else
            Debug.LogError("PauseMenuUI not assigned in Inspector!");

        // Assign the Quit function to the button
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        else
            Debug.LogError("Quit Button not assigned in Inspector!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseMenu();
        }

        // Check for L key press only if the pause menu is active
        if (isPaused && Input.GetKeyDown(KeyCode.L))
        {
            QuitGame();
        }
    }

    void TogglePauseMenu()
    {
        pauseMenuUI.SetActive(!pauseMenuUI.activeSelf); // Toggle the UI immediately
        isPaused = pauseMenuUI.activeSelf; // Set isPaused based on the UI's new state
        Debug.Log("Pause Menu Toggled: " + isPaused);
    }

    void QuitGame()
    {
        if (!isPaused) return; // Ensure QuitGame is only called when paused

        Debug.Log("Quit button clicked!");

        GameManager.instance.QuitGame();
    }
}